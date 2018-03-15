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
    public partial class DriverAndBeatHours : System.Web.UI.Page
    {
        private string connFSP = "Initial Catalog=fsp;Data Source=octa-prod\\octa,5815;user id=sa;password=C@pt@1n@mer1c@";
        protected void Page_Load(object sender, EventArgs e)
        {
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

        protected void btnGetReport_Click(object sender, EventArgs e)
        {
            GetData();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
        }

        private void GetData()
        {
            List<DriverBeat> dbList = new List<DriverBeat>();
            DateTime dtStart = Convert.ToDateTime(startDT.Value + " 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(endDT.Value + " 23:59:59");
            DateTime dtNow = DateTime.Now;
            if (dtStart > dtNow)
            {
                Response.Write("Selected start date is in future.");
                return;
            }
            if (dtStart.ToShortDateString() == dtNow.ToShortDateString())
            {
                //date is today pull data from primary fsp database
                using (SqlConnection conn = new SqlConnection(connFSP))
                {
                    conn.Open();

                    string SQL = "SELECT DISTINCT b.beatnumber, g.vehicleid, d.LastName + ', ' + d.FirstName as 'Driver' FROM GPSTracking g" +
                        " inner join drivers d on g.DriverID = d.DriverID inner join beats b on g.assignedbeat = b.beatid where g.timestamp between '" +
                        dtStart.ToString() + "' and '" + dtEnd.ToString() + "'" +
                        " and assignedbeat <> '00000000-0000-0000-0000-000000000000' AND b.BeatNumber <> '055-999-test' AND d.LastName <> 'McNutty'" +
                        " ORDER BY b.BeatNumber, g.VehicleID, 'Driver'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DriverBeat db = new DriverBeat();
                        db.Date = dtStart.ToShortDateString();
                        db.Beat = rdr[0].ToString();
                        db.VehicleID = rdr[1].ToString();
                        db.Driver = rdr[2].ToString();
                        dbList.Add(db);
                    }

                    rdr.Close();
                    rdr = null;
                    cmd = null;

                    foreach (DriverBeat db in dbList)
                    {
                        DateTime maxTime = Convert.ToDateTime("01/01/2001 00:00:00");
                        SQL = "SELECT g.VehicleStatus, MIN(g.timeStamp) as 'Min', MAX(g.timeStamp) as 'Max' FROM GPSTracking g INNER JOIN Beats b ON g.AssignedBeat = b.BeatID" +
                            " WHERE b.BeatNumber = '" + db.Beat + "' AND g.VehicleID = '" + db.VehicleID + "'" +
                            " AND [timeStamp] BETWEEN '" + dtStart.ToString() + "' and '" + dtEnd.ToString() + "'" +
                            " AND g.VehicleStatus <> 'Waiting For Driver Login'" +
                            " GROUP BY g.VehicleStatus ORDER BY 'Min'";
                        cmd = new SqlCommand(SQL, conn);
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            if (rdr[0].ToString() == "Roll Out")
                            {
                                db.RollOut = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "Driver Logged On")
                            {
                                db.LogOn = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "On Patrol")
                            {
                                db.OnPatrol = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "Roll In")
                            {
                                db.RollIn = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            db.LogOff = maxTime.AddSeconds(30).ToShortTimeString();
                        }
                        rdr.Close();
                        rdr = null;
                    }

                    conn.Close();
                }
            }
            else
            {
                //date is in past (hopefully), pull data from fspArchive database
                using (SqlConnection conn = new SqlConnection(connFSP))
                {
                    conn.Open();

                    string SQL = "SELECT DISTINCT b.beatnumber, g.vehicleid, d.LastName + ', ' + d.FirstName as 'Driver' FROM fspArchive.dbo.GPSTracking g" +
                        " inner join drivers d on g.DriverID = d.DriverID inner join beats b on g.assignedbeat = b.beatid where g.timestamp between '" +
                        dtStart.ToString() + "' and '" + dtEnd.ToString() + "'" +
                        " and assignedbeat <> '00000000-0000-0000-0000-000000000000' AND b.BeatNumber <> '055-999-test' AND d.LastName <> 'McNutty'" +
                        " ORDER BY b.BeatNumber, g.VehicleID, 'Driver'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DriverBeat db = new DriverBeat();
                        db.Date = dtStart.ToShortDateString();
                        db.Beat = rdr[0].ToString();
                        db.VehicleID = rdr[1].ToString();
                        db.Driver = rdr[2].ToString();
                        dbList.Add(db);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;

                    foreach (DriverBeat db in dbList)
                    {
                        DateTime maxTime = Convert.ToDateTime("01/01/2001 00:00:00");
                        SQL = "SELECT g.VehicleStatus, MIN(g.timeStamp) as 'Min', MAX(g.timeStamp) as 'Max' FROM fspArchive.dbo.GPSTracking g INNER JOIN Beats b ON g.AssignedBeat = b.BeatID" +
                            " WHERE b.BeatNumber = '" + db.Beat + "' AND g.VehicleID = '" + db.VehicleID + "'" +
                            " AND [timeStamp] BETWEEN '" + dtStart.ToString() + "' and '" + dtEnd.ToString() + "'" +
                            " AND g.VehicleStatus <> 'Waiting For Driver Login'" +
                            " GROUP BY g.VehicleStatus ORDER BY 'Min'";
                        cmd = new SqlCommand(SQL, conn);
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            if (rdr[0].ToString() == "Roll Out")
                            {
                                db.RollOut = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "Driver Logged On")
                            {
                                db.LogOn = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "On Patrol")
                            {
                                db.OnPatrol = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "Roll In")
                            {
                                db.RollIn = Convert.ToDateTime(rdr[1].ToString()).ToShortTimeString();
                                if (Convert.ToDateTime(rdr[2]) > maxTime)
                                {
                                    maxTime = Convert.ToDateTime(rdr[2]);
                                }
                            }
                            if (rdr[0].ToString() == "On Lunch")
                            {
                                DateTime dtStartLunch = Convert.ToDateTime(rdr[1].ToString());
                                DateTime dtEndLunch = Convert.ToDateTime(rdr[2].ToString());
                                TimeSpan ts = dtEndLunch - dtStartLunch;
                                db.LunchMinutes = Math.Round(ts.TotalMinutes, 2);
                            }
                            db.LogOff = maxTime.AddSeconds(30).ToShortTimeString();
                        }
                        rdr.Close();
                        rdr = null;
                    }

                    conn.Close();
                }
            }

            foreach (DriverBeat db in dbList)
            {
                
                    DateTime rollout = Convert.ToDateTime("01/01/2001 00:00:00");
                    DateTime RollIn = Convert.ToDateTime("01/01/2001 00:00:00");
                    DateTime OnPatrol = Convert.ToDateTime(db.Date + " " + db.OnPatrol);
                    DateTime LogOff = Convert.ToDateTime(db.Date + " " + db.LogOff);;
                    //driver hours = LogOff - RollOut - Lunch
                    //beat hours = rollin - onpatrol
                    if (db.RollOut != null)
                    {
                        rollout = Convert.ToDateTime(db.Date + " " + db.RollOut);
                    }
                    else if (db.OnPatrol != null)
                    {
                        rollout = Convert.ToDateTime(db.Date + " " + db.OnPatrol);
                    }
                    else
                    {
                        //leave it alone.
                    }

                    if (db.RollIn != null)
                    {
                        RollIn = Convert.ToDateTime(db.Date + " " + db.RollIn);
                    }
                    else
                    {
                        RollIn = Convert.ToDateTime("01/01/2001 00:00:00");
                    }

                    if (rollout == Convert.ToDateTime("01/01/2001 00:00:00"))
                    {
                        db.DriverHours = 0;
                    }
                    else
                    {
                        TimeSpan tsDriverHours = LogOff - rollout;
                        db.DriverHours = Math.Round(((tsDriverHours.TotalMinutes) - db.LunchMinutes) / 60, 2);
                    }

                    if (RollIn == Convert.ToDateTime("01/01/2001 00:00:00"))
                    {
                        db.BeatHours = 0;
                    }
                    else
                    {
                        TimeSpan tsBeatHours = RollIn - OnPatrol;
                        db.BeatHours = Math.Round((tsBeatHours.TotalMinutes) / 60, 2);
                    }
            }
            gvData.DataSource = dbList;
            gvData.DataBind();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=DriverBeatHours.xls");
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
    }

    public class DriverBeat
    {
        public string Date { get; set; }
        public string Beat { get; set; }
        public string VehicleID { get; set; }
        public string Driver { get; set; }
        public string LogOn { get; set; }
        public string RollOut { get; set; }
        public string OnPatrol { get; set; }
        public string RollIn { get; set; }
        public string LogOff { get; set; }
        public double LunchMinutes { get; set; }
        public double DriverHours { get; set; }
        public double BeatHours { get; set; }
    }
}