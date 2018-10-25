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
    public partial class DriverAndBeatHoursNew : System.Web.UI.Page
    {
        private string connFSP = "Initial Catalog=fsp;Data Source=octa-prod\\octa,5815;user id=sa;password=C@pt@1n@mer1c@";
        private List<DriverStatus> Stati = new List<DriverStatus>();
        private List<DriverData> uniqueDrivers = new List<DriverData>();

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
            DateTime dtStart = Convert.ToDateTime(startDT.Value + " 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(endDT.Value + " 23:59:59");
            DateTime dtNow = DateTime.Now;
            if (dtStart > dtNow)
            {
                Response.Write("Selected start date is in future.");
                return;
            }
            LoadData(dtStart, dtEnd);
            DataTable dt = findDistinctData();
            dt.DefaultView.Sort = "LastName ASC";
            gvData.DataSource = dt;
            gvData.DataBind();
        }

        private void LoadData(DateTime dtStart, DateTime dtEnd)
        {
            Stati.Clear();
            using (SqlConnection conn = new SqlConnection(connFSP))
            {
                conn.Open();

                string SQL = "SELECT d.LastName, d.FirstName, ds.VehicleID, ds.Status, ds.TimeStamp, ds.BeatNumber, ds.ScheduleName FROM DriverStatus ds" +
                    " INNER JOIN Drivers d ON ds.DriverName = d.FirstName +  ' ' + d.LastName" +
                    " WHERE TimeStamp BETWEEN '" + dtStart.ToString() + "' AND '" + dtEnd.ToString() + "'" +
                    " AND BeatNumber <> 'NA' AND BeatNumber <> 'Not Assigned'" +
                    " ORDER BY BeatNumber, DriverName, VehicleID, TimeStamp";
                SqlCommand cmd = new SqlCommand(SQL, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DriverStatus ds = new DriverStatus();
                    ds.LastName = rdr[0].ToString();
                    ds.FirstName = rdr[1].ToString();
                    ds.VehicleID = rdr[2].ToString();
                    ds.Status = rdr[3].ToString();
                    ds.timeStamp = Convert.ToDateTime(rdr[4]);
                    ds.BeatNumber = rdr[5].ToString();
                    ds.ScheduleName = rdr[6].ToString();
                    Stati.Add(ds);
                }
                rdr.Close();
                rdr = null;
                cmd = null;
                conn.Close();
            }
        }

        private DataTable findDistinctData()
        {
            uniqueDrivers.Clear();
            var disList = Stati.Distinct(new DriverStatus.Comparer());
            foreach (DriverStatus ds in disList)
            {
                DateTime firstLogOn = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime firstRollout = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime firstOnPatrol = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime lastRollIn = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime lastLogOff = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime lunchTime = Convert.ToDateTime("01/01/2001 00:00:00");
                DateTime breakTime = Convert.ToDateTime("01/01/2001 00:00:00");
                int totalLunch = 0;
                DriverData dd = new DriverData();
                dd.LastName = ds.LastName;
                dd.FirstName = ds.FirstName;
                dd.BeatNumber = ds.BeatNumber;
                dd.VehicleID = ds.VehicleID;
                dd.ScheduleName = ds.ScheduleName;
                uniqueDrivers.Add(dd);
                var dList = from s in Stati
                            where s.LastName == dd.LastName && s.FirstName == dd.FirstName && s.BeatNumber == dd.BeatNumber && s.VehicleID == dd.VehicleID && s.ScheduleName == dd.ScheduleName
                            select s;
                dList = dList.OrderBy(d => d.timeStamp).ToList<DriverStatus>();
                foreach (DriverStatus dsData in dList)
                {
                    //find the correct object in uniqueDrivers and update the data accordingly

                    #region " First Loop "
                    DriverData ddFound = uniqueDrivers.Find(d => (d.LastName == dsData.LastName && d.FirstName == dsData.FirstName) && (d.BeatNumber == dsData.BeatNumber) && (d.VehicleID == dsData.VehicleID) && (d.ScheduleName == dsData.ScheduleName));
                    if (ddFound != null)
                    {
                        switch (dsData.Status.ToUpper())
                        {
                            case "LOG ON":
                                if (ddFound.LogOn == null) //only want to capture the first incident
                                {
                                    ddFound.LogOn = dsData.timeStamp;
                                    firstLogOn = dsData.timeStamp;
                                }
                                break;
                            case "ROLL OUT":
                                if (ddFound.RollOut == null)
                                {
                                    ddFound.RollOut = dsData.timeStamp;
                                    firstRollout = dsData.timeStamp;
                                }
                                break;
                            case "ON PATROL":
                                if (ddFound.OnPatrol == null)
                                {
                                    ddFound.OnPatrol = dsData.timeStamp;
                                    firstOnPatrol = dsData.timeStamp;
                                }
                                if (lunchTime != Convert.ToDateTime("01/01/2001 00:00:00"))
                                {
                                    TimeSpan tsLunch = dsData.timeStamp - lunchTime;
                                    totalLunch += (int)tsLunch.TotalMinutes;
                                    lunchTime = Convert.ToDateTime("01/01/2001 00:00:00");
                                }
                                break;
                            case "ROLL IN":
                                ddFound.RollIn = dsData.timeStamp; //we want the last value for roll in
                                lastRollIn = dsData.timeStamp;
                                break;
                            case "LOG OFF":
                                ddFound.LogOff = dsData.timeStamp; //we want the last value for log off
                                lastLogOff = dsData.timeStamp;
                                break;
                            case "ON BREAK":
                                //breakTime = dsData.timeStamp;
                                break;
                            case "ON LUNCH":
                                lunchTime = dsData.timeStamp;
                                break;
                        }
                    }
                }
                    #endregion
                foreach (DriverData ddFound in uniqueDrivers)
                {
                    #region " need another loop "
                    /*  Keeping these values null rather than putting in fake data
                    if (ddFound.LogOn == null)
                    {
                        
                        
                        if (ddFound.RollOut != null)
                        {
                            ddFound.LogOn = ddFound.RollOut;
                        }
                        else if (firstOnPatrol != null)
                        {
                            ddFound.LogOn = ddFound.OnPatrol;
                        }
                        else
                        {
                            string mo = DateTime.Now.Month.ToString();
                            string day = DateTime.Now.Day.ToString();
                            string year = DateTime.Now.Year.ToString();
                            while (mo.Length < 2)
                            {
                                mo = "0" + mo;
                            }
                            while (day.Length < 2)
                            {
                                day = "0" + day;
                            }
                            firstLogOn = Convert.ToDateTime(mo + "/" + day + "/" + year + " 04:00:00");
                        }
                        ddFound.LogOn = firstLogOn;
                        
                        ddFound.LogOn = Convert.ToDateTime("01/01/2001 00:00:00");
                    }

                    if (ddFound.RollOut == null) //rollout
                    {
                        
                        if (ddFound.LogOn != null)
                        {
                            ddFound.RollOut = ddFound.LogOn;
                        }
                        else if (ddFound.OnPatrol != null)
                        {
                            ddFound.RollOut = ddFound.OnPatrol;
                        }
                        else
                        {
                            string mo = DateTime.Now.Month.ToString();
                            string day = DateTime.Now.Day.ToString();
                            string year = DateTime.Now.Year.ToString();
                            while (mo.Length < 2)
                            {
                                mo = "0" + mo;
                            }
                            while (day.Length < 2)
                            {
                                day = "0" + day;
                            }
                            firstRollout = Convert.ToDateTime(mo + "/" + day + "/" + year + " 04:00:00");
                            ddFound.RollOut = firstRollout;
                         
                            ddFound.RollOut = Convert.ToDateTime("01/01/2001 00:00:00");
                        }
                        
                    }

                    if (ddFound.OnPatrol == null) //On Patrol
                    {
                        if (ddFound.RollOut != null)
                        {
                            ddFound.OnPatrol = ddFound.RollOut;
                        }
                        else if (ddFound.LogOn != null)
                        {
                            ddFound.OnPatrol = ddFound.LogOn;
                        }
                        else
                        {
                            string mo = DateTime.Now.Month.ToString();
                            string day = DateTime.Now.Day.ToString();
                            string year = DateTime.Now.Year.ToString();
                            while (mo.Length < 2)
                            {
                                mo = "0" + mo;
                            }
                            while (day.Length < 2)
                            {
                                day = "0" + day;
                            }
                            firstOnPatrol = Convert.ToDateTime(mo + "/" + day + "/" + year + " 04:00:00");
                            ddFound.OnPatrol = firstOnPatrol;
                        }
                        ddFound.OnPatrol = Convert.ToDateTime("01/01/2001 00:00:00");
                        
                    }

                    if (ddFound.RollIn == null) //Roll In
                    {
                        if (ddFound.LogOff != null)
                        {
                            ddFound.RollIn = ddFound.LogOff;
                        }
                        else if (ddFound.OnPatrol != null)
                        {
                            ddFound.RollIn = ddFound.OnPatrol;
                        }
                        else
                        {
                            string mo = DateTime.Now.Month.ToString();
                            string day = DateTime.Now.Day.ToString();
                            string year = DateTime.Now.Year.ToString();
                            while (mo.Length < 2)
                            {
                                mo = "0" + mo;
                            }
                            while (day.Length < 2)
                            {
                                day = "0" + day;
                            }
                            lastRollIn = Convert.ToDateTime(mo + "/" + day + "/" + year + " 04:00:00");
                            ddFound.RollIn = lastRollIn;
                        }
                        ddFound.RollIn = Convert.ToDateTime("01/01/2001 00:00:00");
                    }

                    if (ddFound.LogOff == null) //Log off
                    {
                        if (ddFound.RollIn != null)
                        {
                            ddFound.LogOff = ddFound.RollIn;
                        }
                        else if (ddFound.OnPatrol != null)
                        {
                            ddFound.LogOff = ddFound.OnPatrol;
                        }
                        else
                        {
                            string mo = DateTime.Now.Month.ToString();
                            string day = DateTime.Now.Day.ToString();
                            string year = DateTime.Now.Year.ToString();
                            while (mo.Length < 2)
                            {
                                mo = "0" + mo;
                            }
                            while (day.Length < 2)
                            {
                                day = "0" + day;
                            }
                            lastLogOff = Convert.ToDateTime(mo + "/" + day + "/" + year + " 04:00:00");
                            ddFound.LogOff = lastLogOff;
                        }
                        ddFound.LogOff = Convert.ToDateTime("01/01/2001 00:00:00");
                    }
                    */
                    #endregion

                    //calculate  Driver Hours : (LogOff - Log On)

                    if (ddFound.LogOff != null && ddFound.LogOn != null)
                    {
                        TimeSpan? tsDriverHours = ddFound.LogOff - ddFound.LogOn;
                        ddFound.DriverHours = ((TimeSpan)tsDriverHours).TotalHours;
                    }
                    else
                    {
                        ddFound.DriverHours = 0.0;

                    }

                    //calculate BeatHour : (On Patrol - Last Roll In)

                    if (ddFound.OnPatrol != null && ddFound.RollIn != null)
                    {
                        TimeSpan? tsBeatHours = ddFound.RollIn - ddFound.OnPatrol;
                        ddFound.BeatHours = ((TimeSpan)tsBeatHours).TotalHours;
                    }
                    else
                    {
                        ddFound.BeatHours = 0.0;
                    }

                    ddFound.LunchMinutes = totalLunch;
                    ddFound.DriverHours = ((ddFound.DriverHours * 60) - totalLunch) / 60;
                }

            }

            foreach (DriverData dd in uniqueDrivers)
            {

                if (dd.LogOn == null)
                {
                    //dd.LogOn = dd.RollOut;
                    dd.DriverHours = 0.0;
                    dd.BeatHours = 0.0;
                }

                if (dd.RollOut == null)
                {
                    //dd.RollOut = dd.OnPatrol;
                    dd.DriverHours = 0.0;
                    dd.BeatHours = 0.0;
                }
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("BeatNumber", Type.GetType("System.String"));
            dt.Columns.Add("VehicleID", Type.GetType("System.String"));
            dt.Columns.Add("LastName", Type.GetType("System.String"));
            dt.Columns.Add("FirstName", Type.GetType("System.String"));
            dt.Columns.Add("Date", Type.GetType("System.String"));
            dt.Columns.Add("LogOn", Type.GetType("System.String"));
            dt.Columns.Add("RollOut", Type.GetType("System.String"));
            dt.Columns.Add("OnPatrol", Type.GetType("System.String"));
            dt.Columns.Add("RollIn", Type.GetType("System.String"));
            dt.Columns.Add("LogOff", Type.GetType("System.String"));
            dt.Columns.Add("LunchMinutes", Type.GetType("System.Int32"));
            dt.Columns.Add("DriverHours", Type.GetType("System.String"));
            dt.Columns.Add("BeatHours", Type.GetType("System.String"));

            foreach (DriverData dd in uniqueDrivers)
            {
                //string DriverHourMin = TimeSpan.FromHours(dd.DriverHours).ToString();
                //string BeatHourMin = TimeSpan.FromHours(dd.BeatHours).ToString();
                string date;
                if (dd.LogOn != null)
                {
                    date = ((DateTime)dd.LogOn).ToShortDateString();
                }
                else
                {
                    date = "No Logon";
                }
                var tsDriverHourMin = TimeSpan.FromHours(dd.DriverHours);
                var tsBeatHourMin = TimeSpan.FromHours(dd.BeatHours);
                string DriverHourMin = string.Format("{0} hrs {1} min", tsDriverHourMin.Hours, tsDriverHourMin.Minutes);
                string BeatHourMin = string.Format("{0} hrs {1} min", tsBeatHourMin.Hours, tsBeatHourMin.Minutes);
                DataRow row = dt.NewRow();
                row["BeatNumber"] = dd.BeatNumber;
                row["VehicleID"] = dd.VehicleID;
                row["LastName"] = dd.LastName;
                row["FirstName"] = dd.FirstName;
                row["Date"] = date;
                if (dd.LogOn != null)
                {
                    row["LogOn"] = ((DateTime)dd.LogOn).ToString("HH:mm");
                }
                else
                {
                    row["LogOn"] = "-";
                }
                if (dd.RollOut != null)
                {
                    row["RollOut"] = ((DateTime)dd.RollOut).ToString("HH:mm");
                }
                else
                {
                    row["RollOut"] = "-";
                }
                if (dd.OnPatrol != null)
                {
                    row["OnPatrol"] = ((DateTime)dd.OnPatrol).ToString("HH:mm");
                }
                else
                {
                    row["OnPatrol"] = "-";
                }
                if (dd.RollIn != null)
                {
                    row["RollIn"] = ((DateTime)dd.RollIn).ToString("HH:mm");
                }
                else
                {
                    row["RollIn"] = "-";
                }
                if (dd.LogOff != null)
                {
                    row["LogOff"] = ((DateTime)dd.LogOff).ToString("HH:mm");
                }
                else
                {
                    row["LogOff"] = "-";
                }
                row["LunchMinutes"] = dd.LunchMinutes;
                //row["DriverHours"] = dd.DriverHours;
                //row["BeatHours"] = dd.BeatHours;
                row["DriverHours"] = DriverHourMin;
                row["BeatHours"] = BeatHourMin;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //used to render excel from gridview
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

    public class DriverStatus
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string VehicleID { get; set; }
        public string Status { get; set; }
        public DateTime timeStamp { get; set; }
        public string BeatNumber { get; set; }
        public string ScheduleName { get; set; }

        public class Comparer : IEqualityComparer<DriverStatus>
        {
            public bool Equals(DriverStatus x, DriverStatus y)
            {
                return x.LastName == y.LastName && x.FirstName == y.FirstName && x.VehicleID == y.VehicleID && x.BeatNumber == y.BeatNumber && x.ScheduleName == y.ScheduleName;
            }

            public int GetHashCode(DriverStatus ds)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (ds.LastName + " " + ds.FirstName ?? "").GetHashCode();
                    hash = hash * 23 + (ds.VehicleID ?? "").GetHashCode();
                    hash = hash * 23 + (ds.BeatNumber ?? "").GetHashCode();
                    hash = hash * 23 + (ds.ScheduleName ?? "").GetHashCode();
                    return hash;
                }
            }
        }
    }

    public class DriverData
    {
        public string BeatNumber { get; set; }
        public string VehicleID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? LogOn { get; set; }
        public DateTime? RollOut { get; set; }
        public DateTime? OnPatrol { get; set; }
        public DateTime? RollIn { get; set; }
        public DateTime? LogOff { get; set; }
        public int LunchMinutes { get; set; }
        public double DriverHours { get; set; }
        public double BeatHours { get; set; }
        public string ScheduleName { get; set; }
    }
}