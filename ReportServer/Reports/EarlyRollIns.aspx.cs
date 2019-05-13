using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class EarlyRollIns : Page
    {
        Classes.ReportData thisReport;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
            thisReport = Classes.Reports.GetReportDataByName("EarlyRollIns");
            if (!Page.IsPostBack)
            {
                DateTime dtCurrent = DateTime.Now;
                string Month = dtCurrent.Month.ToString();
                string Day = dtCurrent.Day.ToString();
                string Year = dtCurrent.Year.ToString();
                while (Month.Length < 2)
                {
                    Month = "0" + Month;
                }
                while (Day.Length < 2)
                {
                    Day = "0" + Day;
                }
                string cDateVal = Month + "/" + Day + "/" + Year;
                startDT.Value = cDateVal;
                endDT.Value = cDateVal;

                LoadDrivers();
                LoadBeats();
            }


        }

        private void LoadBeats()
        {
            try
            {
                ddlBeatNumber.Items.Clear();
                ddlBeatNumber.Items.Add("All");
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    string SQL = "select BeatNumber from Beats where BeatNumber NOT LIKE '%test%' Order by BeatNumber";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ddlBeatNumber.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private void LoadDrivers()
        {
            try
            {
                ddlDrivers.Items.Clear();
                ddlDrivers.Items.Add("All");
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    string SQL = "SELECT LastName + ', ' + FirstName FROM Drivers ORDER BY LastName";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ddlDrivers.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=DriverAlarmComments.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.xls";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gvData.AllowPaging = false;
            gvData.HeaderRow.Style.Add("background-color", "#FFFFFF");
            gvData.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string dtStart = startDT.Value + " 00:00:00";
            string dtEnd = endDT.Value + " 23:59:59";
            DataTable dt = new DataTable();
            string SQL = thisReport.SQL;
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, conn);
                if (thisReport.cmdType == "StoredProcedure")
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (thisReport.parameters != null)
                    {
                        string lastName = string.Empty;
                        string firstName = string.Empty;
                        if (ddlDrivers.Text.ToUpper() != "ALL")
                        {
                            string[] splitter = ddlDrivers.Text.ToString().Split(',');
                            lastName = splitter[0].ToString().Trim();
                            firstName = splitter[1].ToString().Trim();
                        }
                        else
                        {
                            lastName = "ALL";
                            firstName = "ALL";
                        }
                        for (int i = 0; i < thisReport.parameters.Count; i++)
                        {
                            if (thisReport.parameters[i].ToString() == "@DateStart")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtStart);
                            }
                            if (thisReport.parameters[i].ToString() == "@DateEnd")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtEnd);
                            }
                            if (thisReport.parameters[i].ToString() == "@BeatNumber")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), ddlBeatNumber.Text);
                            }

                            if (thisReport.parameters[i].ToString() == "@DriverLastName")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), lastName);
                            }
                            if (thisReport.parameters[i].ToString() == "@DriverFirstName")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), firstName);
                            }
                        }
                    }
                }
                cmd.CommandTimeout = 0;
                SqlDataReader rdr = cmd.ExecuteReader();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    dt.Columns.Add(rdr.GetName(i).ToString(), Type.GetType("System.String"));
                }

                while (rdr.Read())
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        row[i] = rdr[i].ToString();
                    }
                    dt.Rows.Add(row);
                }

                rdr.Close();
                rdr = null;
                conn.Close();
                cmd = null;
                gvData.DataSource = dt;
                gvData.DataBind();
            }
        }
    }

}