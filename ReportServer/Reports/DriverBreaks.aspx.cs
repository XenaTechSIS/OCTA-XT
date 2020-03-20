using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class DriverBreaks : Page
    {
        Classes.ReportData thisReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
            thisReport = Classes.Reports.GetReportDataByName("DriverBreaks");
            if (!Page.IsPostBack)
            {
                GetContractors();
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
                endDT.Value = cDateVal;
                startDT.Value = cDateVal;
            }
        }

        protected void btnGetDrivers_Click(object sender, EventArgs e)
        {
            string dtStart = startDT.Value + " 00:00:00";
            string dtEnd = endDT.Value + " 23:59:59";
            if (string.IsNullOrEmpty(dtStart))
            {
                dtStart = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(dtEnd))
            {
                dtEnd = DateTime.Now.ToString();
            }
            ddlDrivers.Items.Clear();
            ddlDrivers.Items.Add("All");
            string ContractorName = "NA";
            if (ddlContractors.Text != "All")
            {
                ContractorName = ddlContractors.Text;
            }
            string SQL = "GetDriversForBreaks";
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString("PROD")))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", dtStart);
                    cmd.Parameters.AddWithValue("@EndDate", dtEnd);
                    cmd.Parameters.AddWithValue("@ContractCompanyName", ContractorName);
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
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
        }

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            string dtStart = startDT.Value + " 00:00:00";
            string dtEnd = endDT.Value + " 23:59:59";
            if (string.IsNullOrEmpty(dtStart))
            {
                dtStart = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(dtEnd))
            {
                dtEnd = DateTime.Now.ToString();
            }
            string ContractorName = "NA";
            string DriverName = "NA";
            if (ddlContractors.Text.ToUpper() != "ALL")
            {
                ContractorName = ddlContractors.Text;
            }
            if (ddlDrivers.Text.ToUpper() != "ALL")
            {
                string[] splitter = ddlDrivers.Text.Split(',');
                DriverName = splitter[0].ToString();
            }
            string SQL = thisReport.SQL;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", dtStart);
                cmd.Parameters.AddWithValue("@EndDate", dtEnd);
                cmd.Parameters.AddWithValue("@ContractCompanyName", ContractorName);
                cmd.Parameters.AddWithValue("@DriverLastName", DriverName);
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

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=DriverBreakReport.xls");
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

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        private void GetContractors()
        {
            ddlContractors.Items.Clear();
            ddlContractors.Items.Add("All");
            string SQL = "GetContractors";
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ddlContractors.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
        }
    }
}