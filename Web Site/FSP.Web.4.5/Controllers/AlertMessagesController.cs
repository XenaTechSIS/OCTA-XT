using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;

namespace FSP.Web.Controllers
{
    [CustomAuthorization(Roles = "Admin, Dispatcher, CHP")]
    public class AlertMessagesController : Controller
    {
        private readonly FSPDataContext db = new FSPDataContext();

        public ActionResult Alerts()
        {
            ViewBag.Heading = "Current Alarms for " + DateTime.Today.ToString("MMMM dd, yyyy");
            return View();
        }

        public ActionResult ClearAlarm(string id, string alarmType)
        {
            var retValue = string.Empty;
            if (!string.IsNullOrEmpty(alarmType) && !string.IsNullOrEmpty(id))
                using (var service = new TowTruckServiceClient())
                {
                    service.ClearAlarm(id, alarmType);
                    Debug.WriteLine("Alarm Cleared for " + id);
                    retValue = "Thank you! Alarm successfully cleared";
                }

            return Json(retValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DriversAlertComments()
        {
            ViewBag.Heading = " Driver's Alert Comments for " + DateTime.Today.ToString("MMMM dd, yyyy");
            return View();
        }

        public ActionResult ExcuseAlarm(string vehicleNumber, string beatNumber, string alarmType, string driverName, string comments)
        {
            var retValue = string.Empty;
            if (!string.IsNullOrEmpty(alarmType) && !string.IsNullOrEmpty(vehicleNumber))
                using (var service = new TowTruckServiceClient())
                {
                    service.ExcuseAlarm(vehicleNumber, beatNumber, alarmType, driverName, comments);
                    Debug.WriteLine("Alarm Excused for " + vehicleNumber);
                    retValue = "Thank you! Alarm successfully excused";
                }
            return Json(retValue, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 60, VaryByParam = "beat;driver;date")]
        [HttpPost]
        public ActionResult GetAlarmHistory(string beat, string driver, string date, string alarmType, bool? isExcused)
        {
            var returnList = new List<AlarmHistory>();

            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            var end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);

            if (!string.IsNullOrEmpty(date))
            {
                var dtDate = Convert.ToDateTime(date);

                start = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 0, 0, 0);
                end = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 23, 59, 59);
            }

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["fspConnectionString"].ToString()))
            {
                // Create the Command and Parameter objects.
                var command = new SqlCommand("GetAlarmHistory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StartTime", start);
                command.Parameters.AddWithValue("@EndTime", end);

                // Open the connection in a try/catch block.  
                // Create and execute the DataReader, writing the result 
                // set to the console window. 
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                        returnList.Add(new AlarmHistory
                        {
                            BeatNumber = reader[0].ToString(),
                            ContractCompanyName = reader[1].ToString(),
                            VehicleNumber = reader[2].ToString(),
                            DriverName = reader[3].ToString(),
                            AlarmTime = Convert.ToDateTime(reader[4].ToString()),
                            AlarmType = reader[5].ToString(),
                            Comments = reader[6].ToString(),
                            ExcuseTime = reader[7].ToString()
                        });
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }

            if (!string.IsNullOrEmpty(beat))
                returnList = returnList.Where(p => p.BeatNumber == beat).ToList();

            if (!string.IsNullOrEmpty(driver))
                returnList = returnList.Where(p => p.DriverName == driver).ToList();

            if (!string.IsNullOrEmpty(alarmType))
                returnList = returnList.Where(p => p.AlarmType.Replace(" ", "").ToLower() == alarmType.ToLower()).ToList();

            if (isExcused != null)
                if (isExcused == true)
                    returnList = returnList.Where(p => !string.IsNullOrEmpty(p.ExcuseTime)).ToList();
                else
                    returnList = returnList.Where(p => string.IsNullOrEmpty(p.ExcuseTime)).ToList();

            return Json(returnList.OrderBy(p => p.BeatNumber).ThenBy(p => p.ContractCompanyName).ThenBy(p => p.VehicleNumber).ThenBy(p => p.DriverName).ToList(), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60)]
        public ActionResult GetAlarmTypes()
        {
            //LOGON
            //ROLLIN
            //ROLLOUT
            //ONPATROL
            //LOGOFF
            //INCIDENT
            //GPSISSUE
            //STATIONARY

            var alarmTypes = new List<string> {"LOGON", "ROLLIN", "ONPATROL", "LOGOFF", "INCIDENT", "GPSISSUE", "STATIONARY"};
            return Json(alarmTypes, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 10)]
        public ActionResult GetAlerts()
        {
            var alarmStatuses = new AlarmStatus[0];
            var returnList = new List<AlarmStatus>();
            using (var service = new TowTruckServiceClient())
            {
                alarmStatuses = service.GetAllAlarms();
                Debug.WriteLine(alarmStatuses.Count() + " alarms received");
            }

            var isValid = false;
            foreach (var item in alarmStatuses)
            {
                isValid = false;

                if (item.RollInAlarm && item.RollInAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.RollOutAlarm && item.RollOutAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.IncidentAlarm && item.IncidentAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.LogOffAlarm && item.LogOffAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.LogOnAlarm && item.LogOnAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.GPSIssueAlarm && item.GPSIssueAlarmStart.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.OnPatrolAlarm && item.OnPatrolAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.SpeedingAlarm && item.SpeedingTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;
                if (item.OutOfBoundsAlarm && item.OutOfBoundsStartTime.ToString("MM/dd/yyyy") != "01/01/2001")
                    isValid = true;


                if (isValid)
                    returnList.Add(item);
            }

            var alarms = returnList.OrderBy(p => p.BeatNumber).ThenBy(p => p.VehicleNumber).ThenBy(p => p.DriverName).ToList();
            Debug.WriteLine(alarms.Count() + " alarms returned");
            return Json(alarms, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60)]
        public ActionResult GetAllBeats()
        {
            var query = from q in db.vBeats
                orderby q.BeatNumber
                select new
                {
                    Id = q.BeatID.ToString(),
                    Text = q.BeatNumber
                };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60)]
        public ActionResult GetAllDrivers()
        {
            var query = from q in db.Drivers
                orderby q.LastName, q.FirstName
                select new
                {
                    Id = q.DriverID.ToString(),
                    Text = q.LastName + ", " + q.FirstName
                };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDriversAlertComments(string beat, string driver, string date, string alarmType)
        {
            var returnList = new List<DriversAlertComment>();

            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            var end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);

            if (!string.IsNullOrEmpty(date))
            {
                var dtDate = Convert.ToDateTime(date);

                start = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 0, 0, 0);
                end = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 23, 59, 59);
            }

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["fspConnectionString"].ToString()))
            {
                // Create the Command and Parameter objects.
                var command = new SqlCommand("GetEarlyRollIns", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@dtStart", start);
                command.Parameters.AddWithValue("@dtEnd", end);

                // Open the connection in a try/catch block.  
                // Create and execute the DataReader, writing the result 
                // set to the console window. 
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                        returnList.Add(new DriversAlertComment
                        {
                            DriverLastName = reader[0].ToString(),
                            DriverFirstName = reader[1].ToString(),
                            DriverFullName = reader[0] + ", " + reader[1],
                            Datestamp = reader[2].ToString(),
                            Explanation = reader[3].ToString(),
                            CHPLogNumber = reader[4].ToString(),
                            VehicleNumber = reader[5].ToString(),
                            BeatNumber = reader[6].ToString(),
                            ExceptionType = reader[7].ToString()
                        });
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }

            if (!string.IsNullOrEmpty(beat))
                returnList = returnList.Where(p => p.BeatNumber == beat).ToList();

            if (!string.IsNullOrEmpty(driver))
                returnList = returnList.Where(p => p.DriverFullName == driver).ToList();

            if (!string.IsNullOrEmpty(alarmType))
                returnList = returnList.Where(p => p.ExceptionType.Replace(" ", "").ToLower() == alarmType.ToLower()).ToList();

            return Json(returnList.OrderBy(p => p.BeatNumber).ThenBy(p => p.Datestamp).ToList(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult History()
        {
            ViewBag.Heading = "Alarm Detail for " + DateTime.Today.ToString("MMMM dd, yyyy");
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendAlertMessage(string alertMessage, string selectedTrucks)
        {
            var fleetVehicles = new List<FleetVehicle>();
            var users = new List<User>();
            using (var dc = new FSPDataContext())
            {
                fleetVehicles = dc.FleetVehicles.ToList();
                users = dc.Users.ToList();
            }

            using (var service = new TowTruckServiceClient())
            {
                var trucks = new JavaScriptSerializer().Deserialize<IEnumerable<SelectedTruck>>(selectedTrucks);
                foreach (var truck in trucks)
                {
                    var ipAddress = truck.ipAddress;
                    if (string.IsNullOrEmpty(ipAddress))
                    {
                        var tr = fleetVehicles.FirstOrDefault(p => p.VehicleNumber == truck.truckNumber);
                        if (tr != null)
                        {
                            ipAddress = tr.IPAddress;
                        }
                    }

                    if(string.IsNullOrEmpty(ipAddress)) continue;
                    

                    var usr = users.FirstOrDefault(p => p.Email == User.Identity.Name);
                    if(usr == null) continue;

                    var message = new TruckMessage
                    {
                        MessageID = Guid.NewGuid(),
                        MessageText = alertMessage,
                        TruckIP = ipAddress,
                        UserID = usr.UserID,
                        SentTime = DateTime.Now
                    };
                    service.SendMessage(message);
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateAlarm(AlarmHistory alarm)
        {
            var retValue = string.Empty;
            if (alarm != null)
                using (var service = new TowTruckServiceClient())
                {
                    if (alarm.IsExcused)
                        service.ExcuseAlarm(alarm.VehicleNumber, alarm.BeatNumber, alarm.AlarmType, alarm.DriverName, alarm.Comments);

                    else
                        service.UnexcuseAlarm(alarm.VehicleNumber, alarm.BeatNumber, alarm.AlarmType, alarm.DriverName, alarm.Comments);

                    retValue = "Alarm Record Updated";
                }

            return Json(retValue, JsonRequestBehavior.AllowGet);
        }
    }
}