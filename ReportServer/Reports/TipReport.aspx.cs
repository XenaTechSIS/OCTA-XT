using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class TipReport : Page
    {
        private Classes.ReportData thisReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["UserOK"] == null || this.Session["UserOK"].ToString() != "OK")
            {
                this.Response.Redirect("Logon.aspx");
            }
            this.thisReport = Classes.Reports.GetReportDataByName("TipReport");
            if (!this.Page.IsPostBack)
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
                this.endDT.Value = cDateVal;
                this.startDT.Value = cDateVal;
            }
        }

        private void LoadTrucks()
        {
            this.ddlTrucks.Items.Clear();
            this.ddlTrucks.Items.Add("All");
            string dtStart = this.startDT.Value + " 00:00:00";
            string dtEnd = this.endDT.Value + " 23:59:59";
            if (string.IsNullOrEmpty(dtStart))
            {
                dtStart = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(dtEnd))
            {
                dtEnd = DateTime.Now.ToString();
            }
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(this.thisReport.ConnString)))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetTrucks", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@dtStart", dtStart);
                    cmd.Parameters.AddWithValue("@dtEnd", dtEnd);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (!string.IsNullOrEmpty(rdr[0].ToString()))
                        {
                            this.ddlTrucks.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    this.Response.Write(ex.ToString());
                }
            }
        }

        protected void btnLoadTrucks_Click(object sender, EventArgs e)
        {
            this.LoadTrucks();
        }

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            string dtStart = this.startDT.Value + " 00:00:00";
            string dtEnd = this.endDT.Value + " 23:59:59";
            if (string.IsNullOrEmpty(dtStart))
            {
                dtStart = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(dtEnd))
            {
                dtEnd = DateTime.Now.ToString();
            }
            string truckNum = this.ddlTrucks.Text;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(this.thisReport.ConnString)))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetTruckMessages", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@dtStart", dtStart);
                    cmd.Parameters.AddWithValue("@dtEnd", dtEnd);
                    cmd.Parameters.AddWithValue("@truckNum", truckNum);
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
                    this.gvData.DataSource = dt;
                    this.gvData.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.ToString());
            }
        }

        public void ExportExcel()
        {
            this.Response.ClearContent();
            this.Response.Buffer = true;
            this.Response.AddHeader("content-disposition", "attachment;filename=TipReport.xls");
            this.Response.Charset = "";
            this.Response.ContentType = "application/vnd.xls";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            this.gvData.AllowPaging = false;
            this.gvData.HeaderRow.Style.Add("background-color", "#FFFFFF");
            this.gvData.RenderControl(htw);
            this.Response.Write(sw.ToString());
            this.Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            this.ExportExcel();
        }
    }
}