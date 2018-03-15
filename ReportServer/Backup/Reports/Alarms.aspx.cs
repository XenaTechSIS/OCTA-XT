using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace ReportServer.Reports
{
    public partial class Alarms : System.Web.UI.Page
    {
        Classes.ReportData thisReport;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
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
            }
            thisReport = Classes.Reports.GetReportDataByName("Alarms");
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=SchedulesReport.xls");
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
                        for (int i = 0; i < thisReport.parameters.Count; i++)
                        {
                            if (thisReport.parameters[i].ToString() == "@StartTime")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtStart);
                            }
                            if (thisReport.parameters[i].ToString() == "@EndTime")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtEnd);
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

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

    }
}