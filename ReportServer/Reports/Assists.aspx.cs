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
    public partial class Assists : System.Web.UI.Page
    {

        Classes.ReportData thisReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
            thisReport = Classes.Reports.GetReportDataByName("Assists");
            if (!Page.IsPostBack)
            {
                LoadIncidentTypes();
                LoadContractors();
                LoadBeats();
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

        private void LoadBeats()
        {
            ddlBeats.Items.Clear();
            ddlBeats.Items.Add("All");
            try
            {
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    string SQL = "SELECT BeatNumber FROM Beats WHERE BeatNumber NOT LIKE '%test%' ORDER BY BeatNumber";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ddlBeats.Items.Add(rdr["BeatNumber"].ToString());
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

        private void LoadIncidentTypes()
        {
            ddlAssistTypes.Items.Clear();
            ddlAssistTypes.Items.Add("All");
            try
            {
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetIncidentTypesForReport", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ddlAssistTypes.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                ddlAssistTypes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        private void LoadContractors()
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
            ddlContractors.SelectedIndex = 0;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        public void ExportExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Assists.xls");
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
            string Contractor = "All";
            string IncidentType = "All";
            string Beat = "All";
            if (!String.IsNullOrEmpty(ddlContractors.Text) && ddlContractors.Text != "All")
            {
                Contractor = ddlContractors.Text;
            }
            if (!String.IsNullOrEmpty(ddlAssistTypes.Text) && ddlAssistTypes.Text != "All")
            {
                IncidentType = ddlAssistTypes.Text;
            }
            if (!String.IsNullOrEmpty(ddlBeats.Text) && ddlBeats.Text != "All")
            {
                Beat = ddlBeats.Text;
            }
            try
            {
                DataTable dt = new DataTable();
                string SQL = "GetAssistDataReport";
                using (SqlConnection conn = new SqlConnection(Classes.SQLConn.GetConnString(thisReport.ConnString)))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dtStart", dtStart);
                    cmd.Parameters.AddWithValue("@dtEnd", dtEnd);
                    cmd.Parameters.AddWithValue("@IncidentType", IncidentType);
                    cmd.Parameters.AddWithValue("@Contractor", Contractor);
                    cmd.Parameters.AddWithValue("@Beat", Beat);
                    dt.Columns.Add("Service Date", Type.GetType("System.String"));
                    dt.Columns.Add("Dispatch Time", Type.GetType("System.String"));
                    dt.Columns.Add("Start Time", Type.GetType("System.String"));
                    dt.Columns.Add("End Time", Type.GetType("System.String"));
                    dt.Columns.Add("Customer Wait Time", Type.GetType("System.String"));
                    dt.Columns.Add("Beat Number", Type.GetType("System.String"));
                    dt.Columns.Add("Incident Number", Type.GetType("System.String"));
                    dt.Columns.Add("Assist Number", Type.GetType("System.String"));
                    dt.Columns.Add("Contractor", Type.GetType("System.String"));
                    dt.Columns.Add("Truck Number", Type.GetType("System.String"));
                    dt.Columns.Add("Driver Name", Type.GetType("System.String"));
                    dt.Columns.Add("Assist Location (Segment #)", Type.GetType("System.String"));
                    dt.Columns.Add("Freeway", Type.GetType("System.String"));
                    dt.Columns.Add("Dir", Type.GetType("System.String"));
                    dt.Columns.Add("Dispatch Location Info", Type.GetType("System.String"));
                    dt.Columns.Add("Lat", Type.GetType("System.String"));
                    dt.Columns.Add("Lon", Type.GetType("System.String"));
                    dt.Columns.Add("Vehicle Type Code", Type.GetType("System.String"));
                    dt.Columns.Add("Color", Type.GetType("System.String"));
                    dt.Columns.Add("License Plate", Type.GetType("System.String"));
                    dt.Columns.Add("Incident Type", Type.GetType("System.String"));
                    dt.Columns.Add("Assist Type", Type.GetType("System.String"));
                    dt.Columns.Add("Vehicle Position", Type.GetType("System.String"));
                    dt.Columns.Add("Traffic Speed", Type.GetType("System.String"));
                    dt.Columns.Add("Tow Location", Type.GetType("System.String"));
                    dt.Columns.Add("Drop Zone", Type.GetType("System.String"));
                    dt.Columns.Add("Comments", Type.GetType("System.String"));

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            if (i < 13)
                            {
                                row[i] = rdr[i].ToString();
                            }
                            else if (i == 13)
                            {
                                string[] splitter = rdr[i].ToString().Split(' ');
                                if (splitter.Length > 3)
                                {
                                    row[13] = splitter[0].ToString();
                                    string colData = "";
                                    for (int iRecord = 2; iRecord < splitter.Length; iRecord++)
                                    {
                                        colData += splitter[iRecord].ToString() + " ";
                                    }
                                    colData = colData.Substring(0,colData.Length - 1);
                                    row[14] = colData ;
                                }
                                else
                                {
                                    row[13] = rdr[i].ToString();
                                    row[14] = rdr[i].ToString();
                                }
                            }
                            else
                            {
                                row[i + 1] = rdr[i].ToString();
                            }
                        }
                        dt.Rows.Add(row);
                    }

                    //SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //da.Fill(dt);
                    //da = null;
                    if (ddlSortBy.Text == "Contractors")
                    {
                        dt.DefaultView.Sort = "Contractor, Service Date, Incident Number, Assist Number ASC";
                    }

                    else if (ddlSortBy.Text == "Assist Types")
                    {
                        dt.DefaultView.Sort = "Assist Type, Service Date, Incident Number, Assist Number ASC";
                    }

                    else if (ddlSortBy.Text == "Incident Types")
                    {
                        dt.DefaultView.Sort = "IncidentType, Service Date, Incident Number, Assist Number ASC";
                    }

                    else
                    {
                        dt.DefaultView.Sort = "Service Date ASC";
                    }

                    gvData.DataSource = dt;
                    gvData.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
    }
}