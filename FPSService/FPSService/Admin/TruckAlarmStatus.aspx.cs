using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPSService.Admin
{
    public partial class TruckAlarmStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logon"] == null)
            {
                Response.Redirect("Logon.aspx");
            }
            string logon = Session["Logon"].ToString();
            if (logon != "true")
            {
                Response.Redirect("Logon.aspx");
            }
            if (Request.QueryString == null)
            {
                Response.Redirect("Dashboard.aspx");
            }
            string ip = Request.QueryString["ip"];
            if (string.IsNullOrEmpty(ip))
            {
                Response.Redirect("Dashboard.aspx");
            }

            if (!Page.IsPostBack)
            {
                LoadTruckData();
            }
        }

        private void LoadTruckData()
        {
            string ip = Request.QueryString["ip"];
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                lblTruck.Text = thisTruck.Extended.TruckNumber;
                lblVehicleNumber.Text = thisTruck.Extended.TruckNumber;
                lblDriverName.Text = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName;
                lblContractCompanyName.Text = thisTruck.Extended.ContractorName;
                lblVehicleStatus.Text = thisTruck.Status.VehicleStatus;
                lblStatusStarted.Text = thisTruck.Status.StatusStarted.ToString();
                //Check for alarm status, set green or red accordingly.
                if (thisTruck.Status.SpeedingAlarm == true)
                {
                    lblSpeedingAlarm.ForeColor = System.Drawing.Color.Red;
                    lblSpeedingValue.ForeColor = System.Drawing.Color.Red;
                    lblSpeedingTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblSpeedingAlarm.ForeColor = System.Drawing.Color.Green;
                    lblSpeedingValue.ForeColor = System.Drawing.Color.Green;
                    lblSpeedingTime.ForeColor = System.Drawing.Color.Green;
                }
                lblSpeedingAlarm.Text = thisTruck.Status.SpeedingAlarm.ToString();
                lblSpeedingValue.Text = thisTruck.Status.SpeedingValue.ToString();
                lblSpeedingTime.Text = thisTruck.Status.SpeedingTime.ToString();

                if (thisTruck.Status.OutOfBoundsAlarm == true)
                {
                    lblOutOfBoundsAlarm.ForeColor = System.Drawing.Color.Red;
                    lblOutOfBoundsMessage.ForeColor = System.Drawing.Color.Red;
                    lblOutOfBoundsTime.ForeColor = System.Drawing.Color.Red;
                    lblOutOfBoundsStartTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblOutOfBoundsAlarm.ForeColor = System.Drawing.Color.Green;
                    lblOutOfBoundsMessage.ForeColor = System.Drawing.Color.Green;
                    lblOutOfBoundsTime.ForeColor = System.Drawing.Color.Green;
                    lblOutOfBoundsStartTime.ForeColor = System.Drawing.Color.Green;
                }

                lblOutOfBoundsAlarm.Text = thisTruck.Status.OutOfBoundsAlarm.ToString();
                lblOutOfBoundsMessage.Text = thisTruck.Status.OutOfBoundsMessage;
                lblOutOfBoundsTime.Text = thisTruck.Status.OutOfBoundsTime.ToString();
                lblOutOfBoundsStartTime.Text = thisTruck.Status.OutOfBoundsStartTime.ToString();

                if (thisTruck.Status.LogOnAlarm == true)
                {
                    lblLogOnAlarm.ForeColor = System.Drawing.Color.Red;
                    lblLogOnAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblLogOnAlarm.ForeColor = System.Drawing.Color.Green;
                    lblLogOnAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblLogOnAlarm.Text = thisTruck.Status.LogOnAlarm.ToString();
                lblLogOnAlarmTime.Text = thisTruck.Status.LogOnAlarmTime.ToString();
                lblLogOnAlarmCleared.Text = thisTruck.Status.LogOnAlarmCleared.ToString();
                lblLogOnAlarmExcused.Text = thisTruck.Status.LogOnAlarmExcused.ToString();
                lblLogOnAlarmComments.Text = thisTruck.Status.LogOnAlarmComments;

                if (thisTruck.Status.RollOutAlarm == true)
                {
                    lblRollOutAlarm.ForeColor = System.Drawing.Color.Red;
                    lblRollOutAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblRollOutAlarm.ForeColor = System.Drawing.Color.Green;
                    lblRollOutAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblRollOutAlarm.Text = thisTruck.Status.RollOutAlarm.ToString();
                lblRollOutAlarmTime.Text = thisTruck.Status.RollOutAlarmTime.ToString();
                lblRollOutAlarmCleared.Text = thisTruck.Status.RollOutAlarmCleared.ToString();
                lblRollOutAlarmExcused.Text = thisTruck.Status.RollOutAlarmExcused.ToString();
                lblRollOutAlarmComments.Text = thisTruck.Status.RollOutAlarmComments;

                if (thisTruck.Status.OnPatrolAlarm == true)
                {
                    lblOnPatrolAlarm.ForeColor = System.Drawing.Color.Red;
                    lblOnPatrolAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblOnPatrolAlarm.ForeColor = System.Drawing.Color.Green;
                    lblOnPatrolAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblOnPatrolAlarm.Text = thisTruck.Status.OnPatrolAlarm.ToString();
                lblOnPatrolAlarmTime.Text = thisTruck.Status.OnPatrolAlarmTime.ToString();
                lblOnPatrolAlarmCleared.Text = thisTruck.Status.OnPatrolAlarmCleared.ToString();
                lblOnPatrolAlarmExcused.Text = thisTruck.Status.OnPatrolAlarmExcused.ToString();
                lblOnPatrolAlarmComments.Text = thisTruck.Status.OnPatrolAlarmComments;

                if (thisTruck.Status.RollInAlarm == true)
                {
                    lblRollInAlarm.ForeColor = System.Drawing.Color.Red;
                    lblRollInAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblRollInAlarm.ForeColor = System.Drawing.Color.Green;
                    lblRollInAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblRollInAlarm.Text = thisTruck.Status.RollInAlarm.ToString();
                lblRollInAlarmTime.Text = thisTruck.Status.RollInAlarmTime.ToString();
                lblRollInAlarmCleared.Text = thisTruck.Status.RollInAlarmCleared.ToString();
                lblRollInAlarmExcused.Text = thisTruck.Status.RollInAlarmExcused.ToString();
                lblRollInAlarmComments.Text = thisTruck.Status.RollInAlarmComments;

                if (thisTruck.Status.LogOffAlarm == true)
                {
                    lblLogOffAlarm.ForeColor = System.Drawing.Color.Red;
                    lblLogOffAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblLogOffAlarm.ForeColor = System.Drawing.Color.Green;
                    lblLogOffAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblLogOffAlarm.Text = thisTruck.Status.LogOffAlarm.ToString();
                lblLogOffAlarmTime.Text = thisTruck.Status.LogOffAlarmTime.ToString();
                lblLogOffAlarmCleared.Text = thisTruck.Status.LogOffAlarmCleared.ToString();
                lblLogOffAlarmExcused.Text = thisTruck.Status.LogOffAlarmExcused.ToString();
                lblLogOffAlarmComments.Text = thisTruck.Status.LogOffAlarmComments;

                if (thisTruck.Status.IncidentAlarm == true)
                {
                    lblIncidentAlarm.ForeColor = System.Drawing.Color.Red;
                    lblIncidentAlarmTime.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblIncidentAlarm.ForeColor = System.Drawing.Color.Green;
                    lblIncidentAlarmTime.ForeColor = System.Drawing.Color.Green;
                }

                lblIncidentAlarm.Text = thisTruck.Status.IncidentAlarm.ToString();
                lblIncidentAlarmTime.Text = thisTruck.Status.IncidentAlarmTime.ToString();
                lblIncidentAlarmCleared.Text = thisTruck.Status.IncidentAlarmCleared.ToString();
                lblIncidentAlarmExcused.Text = thisTruck.Status.IncidentAlarmExcused.ToString();
                lblIncidentAlarmComments.Text = thisTruck.Status.IncidentAlarmComments;

                if (thisTruck.Status.GPSIssueAlarm == true)
                {
                    lblGPSIssueAlarm.ForeColor = System.Drawing.Color.Red;
                    lblGPSIssueStart.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblGPSIssueAlarm.ForeColor = System.Drawing.Color.Green;
                    lblGPSIssueStart.ForeColor = System.Drawing.Color.Green;
                }

                lblGPSIssueAlarm.Text = thisTruck.Status.GPSIssueAlarm.ToString();
                lblGPSIssueStart.Text = thisTruck.Status.GPSIssueAlarmStart.ToString();
                lblGPSIssueCleared.Text = thisTruck.Status.GPSIssueAlarmCleared.ToString();
                lblGPSIssueExcused.Text = thisTruck.Status.GPSIssueAlarmExcused.ToString();
                lblGPSIssueComments.Text = thisTruck.Status.GPSIssueAlarmComments;

                if (thisTruck.Status.StationaryAlarm == true)
                {
                    lblStationaryAlarm.ForeColor = System.Drawing.Color.Red;
                    lblStationaryStart.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblStationaryAlarm.ForeColor = System.Drawing.Color.Green;
                    lblStationaryStart.ForeColor = System.Drawing.Color.Green;
                }

                lblStationaryAlarm.Text = thisTruck.Status.StationaryAlarm.ToString();
                lblStationaryStart.Text = thisTruck.Status.StationaryAlarmStart.ToString();
                lblStationaryCleared.Text = thisTruck.Status.StationaryAlarmCleared.ToString();
                lblStationaryExcused.Text = thisTruck.Status.StationaryAlarmExcused.ToString();
                lblStationaryComments.Text = thisTruck.Status.StationaryAlarmComments;
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            LoadTruckData();
            
        }
    }
}