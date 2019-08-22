﻿using ReportServer.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class OvertimeReport : Page
    {
        private Classes.ReportData thisReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["UserOK"] == null || this.Session["UserOK"].ToString() != "OK")
            {
                this.Response.Redirect("Logon.aspx");
            }
            this.thisReport = Classes.Reports.GetReportDataByName("OvertimeReport");
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

        protected void btnGetReport_ClickOld(object sender, EventArgs e)
        {
            var dtStart = this.startDT.Value + " 00:00:00";
            var dtEnd = this.endDT.Value + " 23:59:59";
            if (string.IsNullOrEmpty(dtStart))
            {
                dtStart = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(dtEnd))
            {
                dtEnd = DateTime.Now.ToString();
            }

            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(Classes.SQLConn.GetConnString(this.thisReport.ConnString)))
                {
                    conn.Open();
                    var cmd = new SqlCommand("GetOvertimeReport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@dtStart", dtStart);
                    cmd.Parameters.AddWithValue("@dtEnd", dtEnd);

                    var rdr = cmd.ExecuteReader();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        dt.Columns.Add(rdr.GetName(i).ToString(), Type.GetType("System.String"));
                    }

                    while (rdr.Read())
                    {
                        var row = dt.NewRow();
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

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            var dtStart = DateTime.Now;
            var dtEnd = DateTime.Now;

            if (!string.IsNullOrEmpty(this.startDT.Value))
            {
                dtStart = Convert.ToDateTime(this.startDT.Value + " 00:00:00");
            }
            if (!string.IsNullOrEmpty(this.endDT.Value))
            {
                dtEnd = Convert.ToDateTime(this.endDT.Value + " 23:59:59");
            }

            try
            {
                using (var fspDatabase = new fspEntities())
                {
                    var allBeatSchedules = (from b in fspDatabase.Beats
                                            join bbs in fspDatabase.BeatBeatSchedules on b.BeatID equals bbs.BeatID
                                            join bs in fspDatabase.BeatSchedules on bbs.BeatScheduleID equals bs.BeatScheduleID
                                            select new
                                            {
                                                b.BeatNumber,
                                                bs.ScheduleName,
                                                bs.Logon,
                                                bs.LogOff
                                            }).OrderBy(p => p.Logon).ToList();



                    var incidents = (from i in fspDatabase.Incidents
                                     join d in fspDatabase.Drivers on i.CreatedBy equals d.DriverID
                                     join c in fspDatabase.Contractors on d.ContractorID equals c.ContractorID
                                     where i.DateStamp >= dtStart && i.DateStamp <= dtEnd
                                     select new
                                     {
                                         i.BeatNumber,
                                         i.Location,
                                         Driver = d.LastName + ", " + d.FirstName,
                                         Callsign = d.FSPIDNumber,
                                         Contractor = c.ContractCompanyName,
                                         i.DateStamp,
                                         i.TimeStamp
                                     }).OrderByDescending(p => p.DateStamp).ThenByDescending(p => p.TimeStamp).ToList();


                    var dt = new DataTable();
                    dt.Columns.Add("Beat");
                    dt.Columns.Add("Location");
                    dt.Columns.Add("Driver");
                    dt.Columns.Add("Callsign");
                    dt.Columns.Add("Contractor");
                    dt.Columns.Add("Incident Date");
                    dt.Columns.Add("Incident Time");
                    dt.Columns.Add("Schedule LogOff");
                    dt.Columns.Add("Overtime");

                    foreach (var incident in incidents)
                    {
                        var row = dt.NewRow();

                        row[0] = incident.BeatNumber;
                        row[1] = incident.Location;
                        row[2] = incident.Driver;
                        row[3] = incident.Callsign;
                        row[4] = incident.Contractor;
                        row[5] = Convert.ToDateTime(incident.DateStamp).ToShortDateString();
                        row[6] = incident.TimeStamp;

                        var beatSchedules = allBeatSchedules.Where(p => p.BeatNumber == incident.BeatNumber).ToList();

                        if (!beatSchedules.Any()) continue;

                        var numberOfTimesInsideSchedule = 0;
                        foreach (var bs in beatSchedules)
                        {
                            var logOffHour = bs.LogOff.Hours;
                            var logOffTotalMinutes = bs.LogOff.TotalMinutes;
                            if (logOffHour == 0) logOffTotalMinutes = logOffTotalMinutes + 2400;

                            var incidentTotalMinutes = incident.TimeStamp.Value.TotalMinutes;

                            if (incident.TimeStamp >= bs.Logon && incidentTotalMinutes <= logOffTotalMinutes)
                            {
                                numberOfTimesInsideSchedule++;
                            }
                        }

                        if (numberOfTimesInsideSchedule > 0) continue;

                        var lastSchedule = beatSchedules.LastOrDefault();

                        var excessTime = incident.TimeStamp - lastSchedule.LogOff;
                        var excessTimeInMinutes = Math.Round(excessTime.Value.TotalMinutes);

                        if (excessTimeInMinutes <= 0) continue;

                        row[7] = lastSchedule.LogOff;
                        row[8] = $"{excessTimeInMinutes} minutes";
                        dt.Rows.Add(row);
                    }

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
            this.Response.AddHeader("content-disposition", "attachment;filename=OvertimeReport.xls");
            this.Response.Charset = "";
            this.Response.ContentType = "application/vnd.xls";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
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