using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportServer.Reports
{
    public partial class ReportMain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserOK"] == null || Session["UserOK"].ToString() != "OK")
            {
                Response.Redirect("Logon.aspx");
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (ddlReports.Text == "Assists")
            {
                Response.Redirect("Assists.aspx");
            }
            if (ddlReports.Text == "Driver Breaks")
            {
                Response.Redirect("DriverBreaks.aspx");
            }
            if (ddlReports.Text == "Driver and Beat Hours")
            {
                Response.Redirect("DriverAndBeatHoursNew.aspx");
            }
            if (ddlReports.Text == "Driver and Beat Hours New")
            {
                Response.Redirect("DriverAndBeatHoursNew.aspx");
            }
            if (ddlReports.Text == "GPS Position Report")
            {
                Response.Redirect("GPSPositionReport.aspx");
            }
            if (ddlReports.Text == "Truck Messages Report")
            {
                Response.Redirect("TruckMessages.aspx");
            }
            if (ddlReports.Text == "Schedules Report")
            {
                Response.Redirect("Schedules.aspx");
            }
            if (ddlReports.Text == "Alarms")
            {
                Response.Redirect("Alarms.aspx");
            }
            if (ddlReports.Text == "Driver Alarm Comments") //report used to be called Early Roll Ins, didn't want to have to rename files
            {
                Response.Redirect("EarlyRollIns.aspx");
            }
            if (ddlReports.Text == "Speeding Report")
            {
                Response.Redirect("Speeding.aspx");
            }
            if (ddlReports.Text == "Combined Driver CHP Comment Report")
            {
                Response.Redirect("CombinedReport.aspx");
            }
        }
    }
}