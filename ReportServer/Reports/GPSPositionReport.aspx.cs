using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class GPSPositionReport : Page
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
                endDT.Value = cDateVal;
                startDT.Value = cDateVal;
            }
            thisReport = Classes.Reports.GetReportDataByName("GPSPositionReport");
        }

        public void LoadData()
        {
            string dtStart = startDT.Value + " 00:00:00";
            string dtEnd = endDT.Value + " 23:59:59";
            string vehicleIDs = ddlVehicles.Text;
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
                            if (thisReport.parameters[i].ToString() == "@dtStart")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtStart);
                            }
                            if (thisReport.parameters[i].ToString() == "@dtEnd")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), dtEnd);
                            }
                            if (thisReport.parameters[i].ToString() == "@VehicleIDs")
                            {
                                cmd.Parameters.AddWithValue(thisReport.parameters[i].ToString(), vehicleIDs);
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

        public void LoadTrucks()
        {
            ddlVehicles.Items.Clear();
            string dtStart = startDT.Value + " 00:00:00";
            string dtEnd = endDT.Value + " 23:59:59";
            DataTable dt = new DataTable();
            string SQL = "SELECT DISTINCT VehicleID FROM gpsTracking" +
                " WHERE timeStamp BETWEEN '" + dtStart + "' and '" + dtEnd + "' ORDER BY VehicleID";
            using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ddlVehicles.Items.Add(rdr[0].ToString());
                }
                rdr.Close();
                rdr = null;
                cmd = null;
                conn.Close();
            }
        }

        protected void btnGetVehicles_Click(object sender, EventArgs e)
        {
            LoadTrucks();
        }

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=GPSReport.xls");
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

        public void ExportPDF()
        {
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=UserDetails.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter hw = new HtmlTextWriter(sw);
            //gvData.AllowPaging = false;
            ////gridName.DataBind();
            //gvData.RenderControl(hw);
            //StringReader sr = new StringReader(sw.ToString());
            //Document pdfDoc = new Document(PageSize.A2, 7f, 7f, 7f, 0f);
            //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            //pdfDoc.Open();
            //htmlparser.Parse(sr); pdfDoc.Close();
            //Response.Write(pdfDoc);
            //Response.End(); 
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }
    }

}