using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ReportServer.Reports
{
    public partial class TruckMessages : System.Web.UI.Page
    {

        Classes.ReportData thisReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
            thisReport = Classes.Reports.GetReportDataByName("TruckMessages");
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
                endDT.Value = cDateVal;
                startDT.Value = cDateVal;
            }
        }

        private void LoadTrucks()
        {
            ddlTrucks.Items.Clear();
            ddlTrucks.Items.Add("All");
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
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetTrucks", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dtStart", dtStart);
                    cmd.Parameters.AddWithValue("@dtEnd", dtEnd);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if(!string.IsNullOrEmpty(rdr[0].ToString()))
                        {
                            ddlTrucks.Items.Add(rdr[0].ToString());
                        }
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

        protected void btnLoadTrucks_Click(object sender, EventArgs e)
        {
            LoadTrucks();
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
            string truckNum = ddlTrucks.Text;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetTruckMessages", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
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
                    gvData.DataSource = dt;
                    gvData.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=TruckMessagesReport.xls");
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
    }
}