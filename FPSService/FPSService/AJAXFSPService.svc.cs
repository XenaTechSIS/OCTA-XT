using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using FPSService.TowTruck;
using Microsoft.SqlServer.Types;

namespace FPSService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AJAXFSPService
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        Logging.EventLogger logger = new Logging.EventLogger();

        #region " Truck status, state, position, and list data "

        [OperationContract]
        [WebGet]
        public void KillTruck(string ip)
        {
            DataClasses.GlobalData.RemoveTowTruck(ip);
        }

        [OperationContract]
        [WebGet]
        public List<MiscData.TrucksDrivers> GetDrivers()
        {
            List<MiscData.TrucksDrivers> truckDrivers = new List<MiscData.TrucksDrivers>();
            foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks)
            {
                truckDrivers.Add(new MiscData.TrucksDrivers
                {
                    TruckID = thisTruck.Extended.FleetVehicleID,
                    TruckNumber = thisTruck.TruckNumber,
                    DriverID = thisTruck.Driver.DriverID,
                    DriverName = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName,
                    ContractorName = thisTruck.Extended.ContractorName
                });
            }
            return truckDrivers;
        }

        [OperationContract]
        [WebGet]
        public void PostEarlyRollInReason(string _reason, string _dt, string _log, string _type)
        {
            //this was originally specced to handle only early roll ins and was later adapted to handle mistimed events in general
            //we support late log on, roll out, on patrol, early roll in, and early log off.  All functionality works essentially the
            //same and gets logged into the EarlyRollIns table in the fsp database.
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck;
            thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                //find the driver id
                DateTime dt = Convert.ToDateTime(_dt);
                Guid DriverID = thisTruck.Driver.DriverID;
                Guid BeatID = thisTruck.assignedBeat.BeatID;
                string vehicleID = thisTruck.TruckNumber;
                if (string.IsNullOrEmpty(vehicleID))
                {
                    vehicleID = "BAD";
                }
                if (DriverID != new Guid("00000000-0000-0000-0000-000000000000") && BeatID != new Guid("00000000-0000-0000-0000-000000000000")
                    && !string.IsNullOrEmpty(vehicleID))
                {
                    SQL.SQLCode mySQL = new SQL.SQLCode();
                    mySQL.LogEarlyRollIn(DriverID, _reason, BeatID, vehicleID, dt, _log, _type);
                    thisTruck.earlyRollin = _reason;
                }
            }
        }

        [OperationContract]
        [WebGet]
        public string MakeSurveyNum()
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            return mySQL.GetSurveyNum();
        }

        private string MakeMsgID()
        {
            DateTime dtSeventy = Convert.ToDateTime("01/01/1970 00:00:00");
            TimeSpan tsSpan = DateTime.Now - dtSeventy;
            double ID = tsSpan.TotalMilliseconds;
            Int64 id = Convert.ToInt64(ID);
            return id.ToString();
        }

        [OperationContract]
        [WebGet]
        public void SendMessage(string IPAddr)
        {
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(IPAddr);
            string msg = "<Reboot><Id=" + MakeMsgID() + "></Id></Reboot>";
            if (thisTruck != null)
            {
                thisTruck.SendMessage(msg);
                KillTruck(IPAddr);
            }
        }

        [OperationContract]
        [WebGet]
        public string GetDate()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(DateTime.Now);
            //return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }

        [OperationContract]
        [WebGet]
        public string SetTruckStatus(string Status, string Mac = "0")
        {
            try
            {
                SQL.SQLCode mySQL = new SQL.SQLCode();
                OperationContext context = OperationContext.Current;
                MessageProperties prop = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string ip = endpoint.Address;
                //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
                if (ip == "::1")
                { ip = "127.0.0.1"; }

                if (Mac != null)
                {
                    if (Mac != "0")
                    {
                        ip = Mac.ToUpper();
                    }
                }

                TowTruck.TowTruck thisTruck;
                thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
                bool lateStatusChange = false;
                if (thisTruck == null)
                { return "Couldn't find truck"; }

                else
                {
                    string CurrentStatus = thisTruck.Status.VehicleStatus;
                    if (CurrentStatus != Status)
                    {
                        thisTruck.Status.StatusStarted = DateTime.Now;
                        if (thisTruck.thisSchedule != null)
                        {
                            if (CurrentStatus == "Driver Logged On" && Status == "Roll Out") //driver has logged on and is rolling out, check schedule for okey dokeyness
                            {
                                DateTime RollOutStart = thisTruck.thisSchedule.RollOut.AddMinutes(DataClasses.GlobalData.RollOutLeeway * -1);
                                DateTime RollOutEnd = thisTruck.thisSchedule.RollOut.AddMinutes(DataClasses.GlobalData.RollOutLeeway);
                                if (DateTime.Now < RollOutStart || DateTime.Now > RollOutEnd)
                                {
                                    thisTruck.Status.RollOutAlarm = true;
                                    thisTruck.Status.RollOutAlarmTime = DateTime.Now;
                                    mySQL.LogAlarm("Roll Out", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID, thisTruck.assignedBeat.BeatID);
                                }
                                if (DateTime.Now > RollOutEnd) //truck is late rolling out
                                {
                                    lateStatusChange = true;
                                }
                            }

                            if (CurrentStatus == "Roll Out" && Status == "On Patrol")
                            {
                                DateTime OnPatrolStart = thisTruck.thisSchedule.OnPatrol.AddMinutes(DataClasses.GlobalData.OnPatrollLeeway * -1);
                                DateTime OnPatrolEnd = thisTruck.thisSchedule.OnPatrol.AddMinutes(DataClasses.GlobalData.OnPatrollLeeway);
                                if (DateTime.Now < OnPatrolStart || DateTime.Now > OnPatrolEnd)
                                {
                                    thisTruck.Status.OnPatrolAlarm = true;
                                    thisTruck.Status.OnPatrolAlarmTime = DateTime.Now;
                                    mySQL.LogAlarm("On Patrol", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID, thisTruck.assignedBeat.BeatID);
                                }
                                if (DateTime.Now > OnPatrolEnd) //truck is late going on patrol
                                {
                                    lateStatusChange = true;
                                }
                            }

                            if (CurrentStatus == "On Patrol" && Status == "Roll In")
                            {
                                DateTime RollInStart = thisTruck.thisSchedule.RollIn.AddMinutes(DataClasses.GlobalData.RollInLeeway * -1);
                                DateTime RollInEnd = thisTruck.thisSchedule.RollIn.AddMinutes(DataClasses.GlobalData.RollInLeeway);
                                if (DateTime.Now < RollInStart || DateTime.Now > RollInEnd)
                                {
                                    //thisTruck.Status.RollInAlarm = true;
                                    //thisTruck.Status.RollInAlarmTime = DateTime.Now;
                                    //mySQL.LogAlarm("Roll In", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID);
                                    return "EARLY";
                                }
                            }

                            if (CurrentStatus == "On Patrol" && Status == "Roll In OK") //this handles an ok for an early roll in
                            {
                                DateTime RollInStart = thisTruck.thisSchedule.RollIn.AddMinutes(DataClasses.GlobalData.RollInLeeway * -1);
                                DateTime RollInEnd = thisTruck.thisSchedule.RollIn.AddMinutes(DataClasses.GlobalData.RollInLeeway);
                                if (DateTime.Now < RollInStart || DateTime.Now > RollInEnd)
                                {
                                    thisTruck.Status.RollInAlarm = true;
                                    thisTruck.Status.RollInAlarmTime = DateTime.Now;
                                    mySQL.LogAlarm("Roll In", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID, thisTruck.assignedBeat.BeatID);
                                    //return "EARLY";
                                }
                            }
                        }
                    }
                    if (Status.Contains("Off") && Status != "Off Patrol")
                    {
                        thisTruck.Status.VehicleStatus = "On Patrol";
                        //thisTruck.Driver.BreakStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                    }
                    else
                    {
                        if (CurrentStatus == "On Lunch")
                        {
                            TimeSpan ts = DateTime.Now - thisTruck.Driver.LunchStarted;
                            int LunchTime = Convert.ToInt32(ts.TotalMinutes);
                            //thisTruck.Driver.LunchStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                            //SQL.SQLCode mySQL = new SQL.SQLCode();
                            mySQL.SetBreakTime(thisTruck.Driver.DriverID, "Lunch", LunchTime);
                        }
                        if (CurrentStatus == "On Break")
                        {
                            TimeSpan ts = DateTime.Now - thisTruck.Driver.BreakStarted;
                            int BreakTime = Convert.ToInt32(ts.TotalMinutes);
                            //thisTruck.Driver.BreakStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                            //SQL.SQLCode mySQL = new SQL.SQLCode();
                            mySQL.SetBreakTime(thisTruck.Driver.DriverID, "Break", BreakTime);
                        }
                        thisTruck.Status.VehicleStatus = Status;
                    }
                    if (Status == "On Break")
                    {
                        thisTruck.Driver.BreakStarted = DateTime.Now;
                    }
                    if (Status == "On Lunch")
                    {
                        thisTruck.Driver.LunchStarted = DateTime.Now;
                    }
                    if (Status == "Roll In OK")
                    {
                        thisTruck.Status.VehicleStatus = "Roll In";
                        Status = "Roll In";
                    }
                    DataClasses.GlobalData.UpdateTowTruck(ip, thisTruck);
                    if (thisTruck.Driver != null)
                    {
                        //SQL.SQLCode mySQL = new SQL.SQLCode();
                        //mySQL.LogEvent(thisTruck.Driver.DriverID, Status);
                        string beatNumber = thisTruck.assignedBeat.BeatNumber;
                        if (string.IsNullOrEmpty(beatNumber))
                        {
                            beatNumber = "NA";
                        }
                        if (Status != "Driver Logged On") //we capture the logon in the logon event
                        {
                            mySQL.LogStatusChange(thisTruck.Driver.FirstName + " " + thisTruck.Driver.LastName, thisTruck.Extended.TruckNumber, Status, DateTime.Now, beatNumber);
                        }
                    }
                    if (lateStatusChange == true)
                    {
                        Status += "|late";
                    }
                    else
                    {
                        Status += "|ontime";
                    }

                    return Status;
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return "NOK";
            }
        }

        [OperationContract]
        [WebGet]
        public string GetAllTrucks()
        {
            List<TowTruckData> myTrucks = new List<TowTruckData>();

            foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks)
            {
                bool HasAlarms = false;
                string _DriverName = "";
                if (thisTruck.Driver.LastName != "Not Logged On")
                {
                    _DriverName = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName;
                }
                else
                {
                    _DriverName = "Not Logged On";
                }
                if (thisTruck.Status.OutOfBoundsAlarm == true || thisTruck.Status.SpeedingAlarm == true)
                { HasAlarms = true; }
                myTrucks.Add(new TowTruckData
                {
                    TruckNumber = thisTruck.TruckNumber,
                    IPAddress = thisTruck.Identifier,
                    Direction = thisTruck.GPSPosition.Head.ToString(),
                    Speed = thisTruck.GPSPosition.Speed,
                    Lat = thisTruck.GPSPosition.Lat,
                    Lon = thisTruck.GPSPosition.Lon,
                    Alarms = HasAlarms,
                    VehicleState = thisTruck.Status.VehicleStatus,
                    SpeedingAlarm = thisTruck.Status.SpeedingAlarm,
                    SpeedingValue = thisTruck.Status.SpeedingValue,
                    SpeedingTime = thisTruck.Status.SpeedingTime,
                    OutOfBoundsAlarm = thisTruck.Status.OutOfBoundsAlarm,
                    OutOfBoundsMessage = thisTruck.Status.OutOfBoundsMessage,
                    OutOfBoundsTime = thisTruck.Status.OutOfBoundsTime,
                    Heading = thisTruck.GPSPosition.Head,
                    LastMessage = thisTruck.LastMessage.LastMessageReceived,
                    ContractorName = thisTruck.Extended.ContractorName,
                    DriverName = _DriverName,
                    BeatNumber = thisTruck.assignedBeat.BeatNumber,
                    Location = "coming soon",
                    StatusStarted = thisTruck.Status.StatusStarted
                });
            }
            myTrucks = myTrucks.OrderBy(x => x.TruckNumber).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string TruckData = js.Serialize(myTrucks);
            return TruckData;
        }

        [OperationContract]
        [WebGet]
        public string GetTruckData(string ipaddr)
        {
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ipaddr);
            if (thisTruck != null)
            {
                bool HasAlarms = false;
                if (thisTruck.Status.OutOfBoundsAlarm == true || thisTruck.Status.SpeedingAlarm == true)
                { HasAlarms = true; }
                TowTruck.TowTruckDashboardState thisState = new TowTruck.TowTruckDashboardState();
                thisState.TruckNumber = thisTruck.TruckNumber;
                thisState.IPAddress = thisTruck.Identifier;
                thisState.Direction = thisTruck.GPSPosition.Head;
                thisState.Speed = thisTruck.GPSPosition.Speed;
                thisState.Lat = thisTruck.GPSPosition.Lat;
                thisState.Lon = thisTruck.GPSPosition.Lon;
                thisState.VehicleState = thisTruck.Status.VehicleStatus;
                thisState.Alarms = HasAlarms;
                thisState.SpeedingAlarm = thisTruck.Status.SpeedingAlarm;
                thisState.SpeedingValue = thisTruck.Status.SpeedingValue;
                thisState.SpeedingTime = thisTruck.Status.SpeedingTime;
                thisState.OutOfBoundsAlarm = thisTruck.Status.OutOfBoundsAlarm;
                thisState.OutOfBoundsMessage = thisTruck.Status.OutOfBoundsMessage;
                thisState.OutOfBoundsTime = thisTruck.Status.OutOfBoundsTime;
                thisState.Heading = thisTruck.GPSPosition.Head;
                thisState.LastMessage = thisTruck.LastMessage.LastMessageReceived;
                thisState.ContractorName = thisTruck.Extended.ContractorName;
                thisState.BeatNumber = thisTruck.assignedBeat.BeatNumber;
                thisState.GPSRate = thisTruck.State.GpsRate;
                thisState.Log = thisTruck.State.Log;
                thisState.Version = thisTruck.State.Version;
                thisState.ServerIP = thisTruck.State.ServerIP;
                thisState.SFTPServerIP = thisTruck.State.SFTPServerIP;
                thisState.GPSStatus = thisTruck.GPSPosition.Status.ToString();
                thisState.GPSDOP = thisTruck.GPSPosition.DOP.ToString();
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(thisState);
            }
            else
            { return null; }
        }

        [OperationContract]
        [WebGet]
        public string GetTruckStatus(string Mac = "0")
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }
            }

            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                TowTruck.ClientTruckStatus thisStatus = new TowTruck.ClientTruckStatus();
                thisStatus.TruckNumber = thisTruck.TruckNumber;
                thisStatus.Speed = thisTruck.GPSPosition.Speed;
                thisStatus.TruckStatus = thisTruck.Status.VehicleStatus;
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(thisStatus);
            }
            else
            {
                return null;
            }
        }

        [OperationContract]
        [WebGet]
        public string GetAllState()
        {
            List<TowTruck.DashboardState> AllState = new List<TowTruck.DashboardState>();
            lock (DataClasses.GlobalData.currentTrucks)
            {
                foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks)
                {
                    AllState.Add(new TowTruck.DashboardState
                    {
                        TruckNumber = thisTruck.TruckNumber,
                        IPAddress = thisTruck.Identifier,
                        GPSRate = thisTruck.State.GpsRate,
                        Log = thisTruck.State.Log,
                        Version = thisTruck.State.Version,
                        ServerIP = thisTruck.State.ServerIP,
                        SFTPServerIP = thisTruck.State.SFTPServerIP
                    });
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(AllState);
        }

        [OperationContract]
        [WebGet]
        public string GetTruckPosition(string Mac = "0")
        {
            string DataOut = "";
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }
            }

            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                MiscData.TruckPosition thisPosition = new MiscData.TruckPosition();
                thisPosition.TruckNumber = thisTruck.TruckNumber;
                thisPosition.Lat = thisTruck.GPSPosition.Lat;
                thisPosition.Lon = thisTruck.GPSPosition.Lon;
                thisPosition.Speed = thisTruck.GPSPosition.Speed;
                thisPosition.TruckStatus = thisTruck.Status.VehicleStatus;
                JavaScriptSerializer js = new JavaScriptSerializer();
                DataOut = js.Serialize(thisPosition);
            }
            return DataOut;
        }

        #endregion

        #region " assist data "

        [OperationContract]
        [WebGet]
        public string GetAllIncidents()
        {
            List<MiscData.IncidentView> theseIncidents = new List<MiscData.IncidentView>();
            SQL.SQLCode mySQL = new SQL.SQLCode();
            try
            {
                foreach (MiscData.Incident thisIncident in DataClasses.GlobalData.Incidents)
                {
                    MiscData.IncidentView thisIV = new MiscData.IncidentView();
                    thisIV.IncidentID = thisIncident.IncidentID;
                    thisIV.Direction = DataClasses.GlobalData.FindLocationNameByID(thisIncident.LocationID);
                    thisIV.Freeway = DataClasses.GlobalData.FindFreewayNameByID(thisIncident.FreewayID);
                    thisIV.Location = thisIncident.Location;
                    thisIV.BeatNumber = thisIncident.BeatNumber;
                    thisIV.TimeStamp = thisIncident.TimeStamp.ToString();
                    thisIV.CreatedBy = mySQL.FindUserNameByID(thisIncident.CreatedBy);
                    thisIV.Description = thisIncident.Description;
                    thisIV.IncidentNumber = thisIncident.IncidentNumber;
                    thisIV.CrossStreet1 = thisIncident.CrossStreet1;
                    thisIV.CrossStreet2 = thisIncident.CrossStreet2;
                    thisIV.DispatchNumber = thisIncident.IncidentNumber;
                    theseIncidents.Add(thisIV);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(theseIncidents);
            }
            catch
            {
                return "err";
            }
        }

        [OperationContract]
        [WebGet]
        public string GetAllAssists()
        {
            List<MiscData.AssistView> theseAssists = new List<MiscData.AssistView>();
            SQL.SQLCode mySQL = new SQL.SQLCode();
            try
            {
                foreach (MiscData.Assist thisAssist in DataClasses.GlobalData.Assists)
                {
                    MiscData.AssistView thisAV = new MiscData.AssistView();
                    thisAV.AssistID = thisAssist.AssistID;
                    thisAV.IncidentID = thisAssist.IncidentID;
                    thisAV.Driver = mySQL.FindUserNameByID(thisAssist.DriverID);
                    thisAV.FleetVehicle = DataClasses.GlobalData.FindTruckNumberByID(thisAssist.FleetVehicleID);
                    thisAV.DispatchTime = thisAssist.DispatchTime.ToString();
                    thisAV.CustomerWaitTime = thisAssist.CustomerWaitTime;
                    thisAV.VehiclePosition = DataClasses.GlobalData.FindVehiclePositionNameByID(thisAssist.VehiclePositionID);
                    thisAV.IncidentType = DataClasses.GlobalData.FindIncidentTypeNameByID(thisAssist.IncidentTypeID);
                    thisAV.TrafficSpeed = DataClasses.GlobalData.FindTrafficSpeedNameByID(thisAssist.TrafficSpeedID);
                    //thisAV.ServiceType = DataClasses.GlobalData.FindServiceTypeNameByID(thisAssist.ServiceTypeID);
                    thisAV.DropZone = thisAssist.DropZone;
                    thisAV.Make = thisAssist.Make;
                    thisAV.VehicleType = DataClasses.GlobalData.FindVehicleTypeNameByID(thisAssist.VehicleTypeID);
                    thisAV.Color = thisAssist.Color;
                    thisAV.LicensePlate = thisAssist.LicensePlate;
                    thisAV.State = thisAssist.State;
                    thisAV.StartOD = thisAssist.StartOD;
                    thisAV.EndOD = thisAssist.EndOD;
                    thisAV.TowLocation = DataClasses.GlobalData.FindTowLocationNameByID(thisAssist.TowLocationID);
                    thisAV.Tip = thisAssist.Tip;
                    thisAV.TipDetail = thisAssist.TipDetail;
                    thisAV.CustomerLastName = thisAssist.CustomerLastName;
                    thisAV.Comments = thisAssist.Comments;
                    thisAV.IsMDC = thisAssist.IsMDC;
                    thisAV.x1097 = thisAssist.x1097.ToString();
                    thisAV.x1098 = thisAssist.x1098.ToString();
                    thisAV.Contractor = DataClasses.GlobalData.FindContractorNameByID(thisAssist.ContractorID);
                    thisAV.LogNumber = thisAssist.LogNumber;
                    thisAV.Lat = thisAssist.Lat;
                    thisAV.Lon = thisAssist.Lon;
                    thisAV.Acked = thisAssist.Acked;
                    thisAV.AssistNumber = thisAssist.AssistNumber;
                    theseAssists.Add(thisAV);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(theseAssists);
            }
            catch
            {
                return "err";
            }
        }

        [OperationContract]
        [WebGet]
        public void ClearAssist(string _AssistID)
        {
            Guid AssistID = new Guid(_AssistID);
            DataClasses.GlobalData.RemoveAssist(AssistID);
        }

        [OperationContract]
        [WebGet]
        public void ClearIncident(string _IncidentID)
        {
            Guid IncidentID = new Guid(_IncidentID);
            DataClasses.GlobalData.ClearIndicent(IncidentID);
        }

        private string GenerateNumber(string Type)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            string NextDispatchNumber = mySQL.GetNextIncidentNumber().ToString();
            string Year = DateTime.Now.Year.ToString();
            string Month = DateTime.Now.Month.ToString();
            while (Month.Length < 2)
            {
                Month = "0" + Month;
            }
            string Day = DateTime.Now.Day.ToString();
            while (Day.Length < 2)
            {
                Day = "0" + Day;
            }
            string DispatchNumber = Year + Month + Day + Type + NextDispatchNumber;
            return DispatchNumber;
        }

        [OperationContract]
        [WebInvoke]
        public void PostClientAssist(MiscData.ClientAssist thisNewIncidentAssist)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (thisNewIncidentAssist.MAC != null)
            {
                if (thisNewIncidentAssist.MAC != "0")
                {
                    ip = thisNewIncidentAssist.MAC.ToUpper();
                }
            }

            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            Guid IncidentID;
            Guid AssistID;
            if (thisTruck != null)
            {
                if (thisNewIncidentAssist.AssistID == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    AssistID = new Guid(MakeGuid());
                }
                else
                {
                    AssistID = thisNewIncidentAssist.AssistID;
                }
                if (thisNewIncidentAssist.IncidentID == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    IncidentID = new Guid(MakeGuid());
                }
                else
                {
                    IncidentID = thisNewIncidentAssist.IncidentID;
                }
                DateTime DispatchTime = thisNewIncidentAssist.x1097;
                DateTime x1097 = thisNewIncidentAssist.x1097;
                DateTime x1098 = DateTime.Now;
                if (thisNewIncidentAssist.DispatchTime != DateTime.Parse("01/01/0001 12:00:00 AM"))
                {
                    DispatchTime = thisNewIncidentAssist.x1097;
                    x1097 = thisNewIncidentAssist.x1097;
                    x1098 = DateTime.Now;
                }

                //figure out beat segment
                Guid bsID = new Guid("00000000-0000-0000-0000-000000000000");

                //Generate the GPS Location for SqlGeography
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(4326);
                builder.BeginGeography(OpenGisGeographyType.Point);
                builder.BeginFigure(thisTruck.GPSPosition.Lat, thisTruck.GPSPosition.Lon);
                builder.EndFigure();
                builder.EndGeography();

                //get beatID
                try
                {
                    //Guid BeatID = BeatData.Beats.FindBeatIDByName(thisNewIncidentAssist.BeatNumber);
                    Guid BeatID = thisTruck.assignedBeat.BeatID;
                    if (string.IsNullOrEmpty(thisTruck.beatSegment) || thisTruck.beatSegment == "NA")
                    {
                        bsID = BeatData.Beats.FindBeatSegmentID(BeatID, builder.ConstructedGeography);
                    }
                    else
                    {
                        bsID = BeatData.Beats.FindBeatSegmentIDbyName(BeatID, thisTruck.currentBeatSegment);
                    }
                }
                catch
                { }

                //check to see if we already have an incident with this id
                MiscData.Incident foundIncident = DataClasses.GlobalData.FindIncidentByID(thisNewIncidentAssist.IncidentID);
                if (foundIncident == null)
                {
                    //Create the incident
                    MiscData.Incident thisIncident = new MiscData.Incident();
                    thisIncident.IncidentID = IncidentID;
                    thisIncident.BeatSegmentID = bsID;
                    thisIncident.TimeStamp = DateTime.Now;
                    thisIncident.Description = thisNewIncidentAssist.Description;
                    thisIncident.IncidentNumber = GenerateNumber("i");
                    thisIncident.Location = thisNewIncidentAssist.Location;
                    thisIncident.LocationID = thisNewIncidentAssist.LocationID;
                    thisIncident.FreewayID = thisNewIncidentAssist.FreewayID;
                    thisIncident.CreatedBy = thisNewIncidentAssist.CreatedBy;
                    thisIncident.CrossStreet1 = thisNewIncidentAssist.CrossStreet1;
                    thisIncident.CrossStreet2 = thisNewIncidentAssist.CrossStreet2;
                    thisIncident.Direction = thisNewIncidentAssist.Direction;
                    thisIncident.BeatNumber = thisTruck.assignedBeat.BeatNumber;
                    DataClasses.GlobalData.AddIncident(thisIncident);
                }
                else
                {
                    foundIncident.BeatSegmentID = bsID;
                }

                MiscData.Assist foundAssist = DataClasses.GlobalData.FindAssistByID(thisNewIncidentAssist.AssistID);
                if (foundAssist == null)
                {
                    //Create a new assist, this will generate an assist number, updating the assist won't
                    MiscData.Assist thisAssist = new MiscData.Assist();
                    thisAssist.AssistID = AssistID;
                    thisAssist.IncidentID = IncidentID;
                    thisAssist.DispatchTime = DispatchTime;
                    thisAssist.DriverID = thisNewIncidentAssist.DriverID;
                    thisAssist.IncidentTypeID = thisNewIncidentAssist.IncidentTypeID;
                    thisAssist.FleetVehicleID = thisNewIncidentAssist.FleetVehicleID;
                    thisAssist.TrafficSpeedID = thisNewIncidentAssist.TrafficSpeedID;
                    thisAssist.DropZone = thisNewIncidentAssist.DropZone;
                    thisAssist.Make = thisNewIncidentAssist.Make;
                    thisAssist.VehicleTypeID = thisNewIncidentAssist.VehicleTypeID;
                    thisAssist.VehiclePositionID = thisNewIncidentAssist.VehiclePositionID;
                    thisAssist.Color = thisNewIncidentAssist.Color;
                    thisAssist.LicensePlate = thisNewIncidentAssist.LicensePlate;
                    thisAssist.State = thisNewIncidentAssist.State;
                    thisAssist.StartOD = thisNewIncidentAssist.StartOD;
                    thisAssist.EndOD = thisNewIncidentAssist.EndOD;
                    thisAssist.TowLocationID = thisNewIncidentAssist.TowLocationID;
                    thisAssist.Tip = thisNewIncidentAssist.Tip;
                    thisAssist.TipDetail = thisNewIncidentAssist.TipDetail;
                    thisAssist.CustomerLastName = thisNewIncidentAssist.CustomerLastName;
                    thisAssist.Comments = thisNewIncidentAssist.Comments;
                    thisAssist.IsMDC = thisNewIncidentAssist.IsMDC;
                    thisAssist.x1097 = x1097;
                    //thisAssist.x1098 = thisNewIncidentAssist.x1098;
                    thisAssist.x1098 = DateTime.Now;
                    thisAssist.OnSiteTime = thisNewIncidentAssist.OnSiteTime;
                    thisAssist.ContractorID = thisNewIncidentAssist.ContractorID;
                    thisAssist.LogNumber = thisNewIncidentAssist.LogNumber;
                    thisAssist.Lat = thisTruck.GPSPosition.Lat;
                    thisAssist.CustomerWaitTime = thisNewIncidentAssist.CustomerWaitTime;
                    thisAssist.Lon = thisTruck.GPSPosition.Lon;
                    thisAssist.Acked = true;
                    thisAssist.SelectedServices = thisNewIncidentAssist.SelectedServices;
                    thisAssist.AssistComplete = true;
                    thisAssist.SurveyNum = thisNewIncidentAssist.SurveyNum;
                    thisAssist.AssistNumber = GenerateNumber("a");
                    DataClasses.GlobalData.AddAssist(thisAssist);
                }
                else
                {

                    MiscData.Assist thisAssist = new MiscData.Assist();
                    if (foundAssist.AssistNumber == null)
                    {
                        thisAssist.AssistNumber = GenerateNumber("a");
                    }
                    thisAssist.AssistID = AssistID;
                    thisAssist.IncidentID = IncidentID;
                    thisAssist.DispatchTime = DispatchTime;
                    thisAssist.DriverID = thisNewIncidentAssist.DriverID;
                    thisAssist.IncidentTypeID = thisNewIncidentAssist.IncidentTypeID;
                    thisAssist.FleetVehicleID = thisNewIncidentAssist.FleetVehicleID;
                    thisAssist.TrafficSpeedID = thisNewIncidentAssist.TrafficSpeedID;
                    thisAssist.DropZone = thisNewIncidentAssist.DropZone;
                    thisAssist.Make = thisNewIncidentAssist.Make;
                    thisAssist.VehicleTypeID = thisNewIncidentAssist.VehicleTypeID;
                    thisAssist.VehiclePositionID = thisNewIncidentAssist.VehiclePositionID;
                    thisAssist.Color = thisNewIncidentAssist.Color;
                    thisAssist.LicensePlate = thisNewIncidentAssist.LicensePlate;
                    thisAssist.State = thisNewIncidentAssist.State;
                    thisAssist.StartOD = thisNewIncidentAssist.StartOD;
                    thisAssist.EndOD = thisNewIncidentAssist.EndOD;
                    thisAssist.TowLocationID = thisNewIncidentAssist.TowLocationID;
                    thisAssist.Tip = thisNewIncidentAssist.Tip;
                    thisAssist.TipDetail = thisNewIncidentAssist.TipDetail;
                    thisAssist.CustomerLastName = thisNewIncidentAssist.CustomerLastName;
                    thisAssist.Comments = thisNewIncidentAssist.Comments;
                    thisAssist.IsMDC = thisNewIncidentAssist.IsMDC;
                    thisAssist.x1097 = x1097;
                    //thisAssist.x1098 = thisNewIncidentAssist.x1098;
                    thisAssist.x1098 = DateTime.Now;
                    thisAssist.OnSiteTime = thisNewIncidentAssist.OnSiteTime;
                    thisAssist.ContractorID = thisNewIncidentAssist.ContractorID;
                    thisAssist.LogNumber = thisNewIncidentAssist.LogNumber;
                    thisAssist.Lat = thisTruck.GPSPosition.Lat;
                    thisAssist.CustomerWaitTime = thisNewIncidentAssist.CustomerWaitTime;
                    thisAssist.Lon = thisTruck.GPSPosition.Lon;
                    thisAssist.Acked = true;
                    thisAssist.SelectedServices = thisNewIncidentAssist.SelectedServices;
                    thisAssist.AssistComplete = true;
                    thisAssist.AssistNumber = foundAssist.AssistNumber;
                    thisAssist.SurveyNum = thisNewIncidentAssist.SurveyNum;
                    DataClasses.GlobalData.AddAssist(thisAssist);
                }
            }
        }

        [OperationContract]
        [WebInvoke]
        public void PostClientAssist2(MiscData.ClientAssist thisNewIncidentAssist)
        {
            /*
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
             * */
            Guid IncidentID;
            Guid AssistID;
            //if (thisTruck != null)
            //{
            if (thisNewIncidentAssist.AssistID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                AssistID = new Guid(MakeGuid());
            }
            else
            {
                AssistID = thisNewIncidentAssist.AssistID;
            }
            if (thisNewIncidentAssist.IncidentID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                IncidentID = new Guid(MakeGuid());
            }
            else
            {
                IncidentID = thisNewIncidentAssist.IncidentID;
            }
            DateTime DispatchTime = DateTime.Now;
            DateTime x1097 = thisNewIncidentAssist.x1097;
            DateTime x1098 = thisNewIncidentAssist.x1098;
            if (thisNewIncidentAssist.DispatchTime != DateTime.Parse("01/01/0001 00:00:00"))
            {
                DispatchTime = thisNewIncidentAssist.DispatchTime;
                x1097 = thisNewIncidentAssist.DispatchTime;
                x1098 = thisNewIncidentAssist.DispatchTime;
            }
            else
            {
                DispatchTime = x1097;
            }

            //check to see if we already have an incident with this id
            MiscData.Incident foundIncident = DataClasses.GlobalData.FindIncidentByID(thisNewIncidentAssist.IncidentID);
            if (foundIncident == null)
            {
                //Create the incident
                MiscData.Incident thisIncident = new MiscData.Incident();
                thisIncident.IncidentID = IncidentID;
                thisIncident.BeatSegmentID = thisNewIncidentAssist.BeatSegmentID;
                thisIncident.TimeStamp = DateTime.Now;
                thisIncident.Description = thisNewIncidentAssist.Description;
                thisIncident.IncidentNumber = GenerateNumber("i");
                thisIncident.Location = thisNewIncidentAssist.Location;
                thisIncident.LocationID = thisNewIncidentAssist.LocationID;
                thisIncident.FreewayID = thisNewIncidentAssist.FreewayID;
                thisIncident.CreatedBy = thisNewIncidentAssist.CreatedBy;
                thisIncident.CrossStreet1 = thisNewIncidentAssist.CrossStreet1;
                thisIncident.CrossStreet2 = thisNewIncidentAssist.CrossStreet2;
                thisIncident.Direction = thisNewIncidentAssist.Direction;
                thisIncident.BeatNumber = thisNewIncidentAssist.BeatNumber;
                DataClasses.GlobalData.AddIncident(thisIncident);
            }

            MiscData.Assist foundAssist = DataClasses.GlobalData.FindAssistByID(thisNewIncidentAssist.AssistID);
            if (foundAssist == null)
            {
                //Create a new assist, this will generate an assist number, updating the assist won't
                MiscData.Assist thisAssist = new MiscData.Assist();
                thisAssist.AssistID = AssistID;
                thisAssist.IncidentID = IncidentID;
                thisAssist.DispatchTime = DispatchTime;
                thisAssist.DriverID = thisNewIncidentAssist.DriverID;
                thisAssist.IncidentTypeID = thisNewIncidentAssist.IncidentTypeID;
                thisAssist.FleetVehicleID = thisNewIncidentAssist.FleetVehicleID;
                thisAssist.TrafficSpeedID = thisNewIncidentAssist.TrafficSpeedID;
                thisAssist.DropZone = thisNewIncidentAssist.DropZone;
                thisAssist.Make = thisNewIncidentAssist.Make;
                thisAssist.VehicleTypeID = thisNewIncidentAssist.VehicleTypeID;
                thisAssist.VehiclePositionID = thisNewIncidentAssist.VehiclePositionID;
                thisAssist.Color = thisNewIncidentAssist.Color;
                thisAssist.LicensePlate = thisNewIncidentAssist.LicensePlate;
                thisAssist.State = thisNewIncidentAssist.State;
                thisAssist.StartOD = thisNewIncidentAssist.StartOD;
                thisAssist.EndOD = thisNewIncidentAssist.EndOD;
                thisAssist.TowLocationID = thisNewIncidentAssist.TowLocationID;
                thisAssist.Tip = thisNewIncidentAssist.Tip;
                thisAssist.TipDetail = thisNewIncidentAssist.TipDetail;
                thisAssist.CustomerLastName = thisNewIncidentAssist.CustomerLastName;
                thisAssist.Comments = thisNewIncidentAssist.Comments;
                thisAssist.IsMDC = thisNewIncidentAssist.IsMDC;
                thisAssist.x1097 = x1097;
                thisAssist.x1098 = thisNewIncidentAssist.x1098;
                thisAssist.OnSiteTime = thisNewIncidentAssist.OnSiteTime;
                thisAssist.ContractorID = thisNewIncidentAssist.ContractorID;
                thisAssist.LogNumber = thisNewIncidentAssist.LogNumber;
                thisAssist.Lat = 0.0;
                thisAssist.CustomerWaitTime = thisNewIncidentAssist.CustomerWaitTime;
                thisAssist.Lon = 0.0;
                thisAssist.Acked = true;
                thisAssist.SelectedServices = thisNewIncidentAssist.SelectedServices;
                thisAssist.AssistComplete = true;
                thisAssist.SurveyNum = thisNewIncidentAssist.SurveyNum;
                thisAssist.AssistNumber = GenerateNumber("a");
                DataClasses.GlobalData.AddAssist(thisAssist);
            }
            else
            {

                MiscData.Assist thisAssist = new MiscData.Assist();
                if (foundAssist.AssistNumber == null)
                {
                    thisAssist.AssistNumber = GenerateNumber("a");
                }
                thisAssist.AssistID = AssistID;
                thisAssist.IncidentID = IncidentID;
                thisAssist.DispatchTime = DispatchTime;
                thisAssist.DriverID = thisNewIncidentAssist.DriverID;
                thisAssist.IncidentTypeID = thisNewIncidentAssist.IncidentTypeID;
                thisAssist.FleetVehicleID = thisNewIncidentAssist.FleetVehicleID;
                thisAssist.TrafficSpeedID = thisNewIncidentAssist.TrafficSpeedID;
                thisAssist.DropZone = thisNewIncidentAssist.DropZone;
                thisAssist.Make = thisNewIncidentAssist.Make;
                thisAssist.VehicleTypeID = thisNewIncidentAssist.VehicleTypeID;
                thisAssist.VehiclePositionID = thisNewIncidentAssist.VehiclePositionID;
                thisAssist.Color = thisNewIncidentAssist.Color;
                thisAssist.LicensePlate = thisNewIncidentAssist.LicensePlate;
                thisAssist.State = thisNewIncidentAssist.State;
                thisAssist.StartOD = thisNewIncidentAssist.StartOD;
                thisAssist.EndOD = thisNewIncidentAssist.EndOD;
                thisAssist.TowLocationID = thisNewIncidentAssist.TowLocationID;
                thisAssist.Tip = thisNewIncidentAssist.Tip;
                thisAssist.TipDetail = thisNewIncidentAssist.TipDetail;
                thisAssist.CustomerLastName = thisNewIncidentAssist.CustomerLastName;
                thisAssist.Comments = thisNewIncidentAssist.Comments;
                thisAssist.IsMDC = thisNewIncidentAssist.IsMDC;
                thisAssist.x1097 = x1097;
                thisAssist.x1098 = thisNewIncidentAssist.x1098;
                thisAssist.OnSiteTime = thisNewIncidentAssist.OnSiteTime;
                thisAssist.ContractorID = thisNewIncidentAssist.ContractorID;
                thisAssist.LogNumber = thisNewIncidentAssist.LogNumber;
                thisAssist.Lat = 0.0;
                thisAssist.CustomerWaitTime = thisNewIncidentAssist.CustomerWaitTime;
                thisAssist.Lon = 0.0;
                thisAssist.Acked = true;
                thisAssist.SelectedServices = thisNewIncidentAssist.SelectedServices;
                thisAssist.AssistComplete = true;
                thisAssist.AssistNumber = foundAssist.AssistNumber;
                thisAssist.SurveyNum = thisNewIncidentAssist.SurveyNum;
                DataClasses.GlobalData.AddAssist(thisAssist);
            }
            //}
        }

        [OperationContract]
        [WebInvoke]
        public void PostAssist(MiscData.ClientAssist thisASsist)
        {


        }

        [OperationContract]
        [WebGet]
        public string MakeGuid()
        {
            Guid g;
            g = Guid.NewGuid();
            return g.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetAssistRequestDetail(string _AssistID, string Mac = "0")
        {
            string Return = "NOT FOUND";
            Guid RequestedID = new Guid(_AssistID);
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }

            }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                foreach (MiscData.Assist thisRequest in thisTruck.theseRequests)
                {
                    if (thisRequest.AssistID == RequestedID)
                    {
                        MiscData.ClientAssistData thisCA = new MiscData.ClientAssistData();
                        MiscData.Incident thisIncident = DataClasses.GlobalData.FindIncidentByID(thisRequest.IncidentID);
                        thisCA.IncidentID = thisIncident.IncidentID;
                        thisCA.AssistID = thisRequest.AssistID;
                        thisCA.Freeway = DataClasses.GlobalData.FindFreewayNameByID(thisIncident.FreewayID);
                        thisCA.Location = thisIncident.Location;
                        thisCA.CrossStreet = thisIncident.CrossStreet1 + " AND " + thisIncident.CrossStreet2;
                        thisCA.Direction = thisIncident.Direction;
                        thisCA.Comments = thisRequest.Comments;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Return = js.Serialize(thisCA);
                    }
                }
            }
            return Return;
        }

        [OperationContract]
        [WebGet]
        public void AckAssistRequest(string _AssistID)
        {
            Guid AssistID = new Guid(_AssistID);
            DataClasses.GlobalData.AckAssistRequest(AssistID);
        }

        [OperationContract]
        [WebGet]
        public string GetAssistRequests(string Mac = "0")
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }

            }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                List<MiscData.PortableAssistRequest> theseRequests = new List<MiscData.PortableAssistRequest>();

                List<MiscData.Assist> truckAssistRequests = DataClasses.GlobalData.GetAssistsByTruck(thisTruck.Extended.FleetVehicleID);
                BeatData.Beat thisBeat = new BeatData.Beat();
                foreach (MiscData.Assist req in truckAssistRequests)
                {
                    if (req.Acked == false)
                    {
                        if (req.x1098 == DateTime.Parse("01/01/0001 00:00:00"))
                        {
                            //string beatsegArea = "";
                            try
                            {
                                //SqlGeography thisGeo = BeatData.Beats.FindBeatSegmentByID(req.BeatSegmentID);
                                //beatsegArea = thisGeo.ToString();
                                string IncidentDetail = "";
                                foreach (MiscData.Incident thisIncident in DataClasses.GlobalData.Incidents)
                                {
                                    if (thisIncident.IncidentID == req.IncidentID)
                                    {
                                        IncidentDetail = thisIncident.Description;
                                        IncidentDetail += Environment.NewLine + "Direction: " + thisIncident.Direction + " Freeway: " + DataClasses.GlobalData.FindFreewayNameByID(thisIncident.FreewayID);
                                        if (!string.IsNullOrEmpty(thisIncident.CrossStreet1))
                                        {
                                            IncidentDetail += Environment.NewLine + "Near: " + thisIncident.CrossStreet1;
                                        }
                                        if (!string.IsNullOrEmpty(thisIncident.CrossStreet2))
                                        {
                                            IncidentDetail += " and " + thisIncident.CrossStreet2;
                                        }
                                        IncidentDetail += Environment.NewLine + req.Comments;
                                    }
                                }
                                theseRequests.Add(new MiscData.PortableAssistRequest
                                {
                                    IncidentID = req.IncidentID,
                                    AssistID = req.AssistID,
                                    DispatchTime = req.DispatchTime,
                                    AssistInfo = IncidentDetail,
                                    CreatedByID = DataClasses.GlobalData.FindIncidentCreatorFromAssist(req.IncidentID),
                                    Description = IncidentDetail,

                                    //AssistType = req.AssistType,
                                    //BeatBoundry = beatsegArea
                                });
                            }
                            catch
                            { }
                        }
                    }
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(theseRequests);
            }
            else
            { return "No such truck"; }
        }

        #endregion

        #region " incident data get, set, clear "

        public void PostIncident(MiscData.Incident thisIncident)
        {
            DataClasses.GlobalData.AddIncident(thisIncident);
        }

        #endregion

        #region " logon, logoff, breaktime, and assorted driver operations "

        [OperationContract]
        [WebGet]
        public void LogOffDriver(string _DriverID)
        {
            Guid DriverID = new Guid(_DriverID);
            DataClasses.GlobalData.ForceDriverLogoff(DriverID);
        }

        [OperationContract]
        [WebGet]
        public string GetDriver()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck;
            thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck == null)
            { return "Couldn't find Driver"; }
            else
            {
                TowTruck.TowTruckDriver thisDriver = thisTruck.Driver;
                JavaScriptSerializer js = new JavaScriptSerializer();
                string driverInfo = js.Serialize(thisDriver);
                return driverInfo;
            }
        }

        [OperationContract]
        [WebGet]
        public void SetBeat(string AssignedBeat, string Mac = "0")
        {
            try
            {
                OperationContext context = OperationContext.Current;
                MessageProperties prop = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string ip = endpoint.Address;
                //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
                if (ip == "::1")
                { ip = "127.0.0.1"; }

                if (Mac != null)
                {
                    if (Mac != "0")
                    {
                        ip = Mac.ToUpper();
                    }
                }

                TowTruck.TowTruck thisTruck;
                thisTruck = DataClasses.GlobalData.FindTowTruck(ip);

                if (!string.IsNullOrEmpty(AssignedBeat))
                {
                    BeatData.Beat thisBeat = BeatData.Beats.GetDriverBeat(new Guid(AssignedBeat));
                    if (thisBeat != null)
                    {
                        thisTruck.assignedBeat.BeatID = thisBeat.BeatID;
                        thisTruck.assignedBeat.BeatNumber = thisBeat.BeatNumber;
                        thisTruck.assignedBeat.BeatExtent = thisBeat.BeatExtent;
                        thisTruck.assignedBeat.Loaded = true;
                    }
                    else
                    {
                        thisTruck.assignedBeat.BeatID = new Guid("00000000-0000-0000-0000-000000000000");
                        thisTruck.assignedBeat.Loaded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToShortDateString() + Environment.NewLine + "Error switching beats" + Environment.NewLine + ex.ToString(), true);
            }
        }

        [OperationContract]
        [WebGet]
        public string DriverLogon(string FSPIDNumber, string Password, string AssignedBeat, string Mac, string _forceLogOn = "F")
        {
            string ip = "";
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }
            }

            //If TestDriver do the magic truck login dance ... OhmChe, OhmChe, OhmChe
            if (FSPIDNumber == "TestDriver" && Password == "testdriver")
            {

                if (Mac != null)
                {
                    if (Mac != "0")
                    {
                        ip = Mac;
                    }
                }

                TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
                GPS thisGPS = null;

                //thisGPS = js.Deserialize<TowTruck.GPS>(msg);
                XmlSerializer ser = new XmlSerializer(typeof(TowTruck.GPS));
                StringReader rdr = new StringReader("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<GPS>\r\n  <Id>229</Id>\r\n  <Speed>55</Speed>\r\n  <Lat>33.775141495952752</Lat>\r\n  <Lon>-117.85245454410458</Lon>\r\n  <MaxSpd>65</MaxSpd>\r\n  <MLat>12.7866</MLat>\r\n  <MLon>-42.909878</MLon>\r\n  <Time>2017-07-21T10:44:26</Time>\r\n  <Status>Valid</Status>\r\n  <DOP>7</DOP>\r\n  <Alt>5280</Alt>\r\n  <Head>331</Head>\r\n</GPS>");
                thisGPS = (TowTruck.GPS)ser.Deserialize(rdr);
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(4326);
                builder.BeginGeography(OpenGisGeographyType.Point);
                builder.BeginFigure(thisGPS.Lat, thisGPS.Lon);
                builder.EndFigure();
                builder.EndGeography();
                thisGPS.Position = builder.ConstructedGeography;

                if (thisTruck != null)
                {
                    try
                    {
                        thisTruck.LastMessage.LastMessageReceived = DateTime.Now;
                        DataClasses.GlobalData.UpdateTowTruck(ip, thisTruck);
                        //DataClasses.GlobalData.AddTowTruck(thisTruck);
                        if (string.IsNullOrEmpty(thisTruck.Extended.TruckNumber))
                        {
                            SQL.SQLCode mySQL2 = new SQL.SQLCode();
                            TowTruck.TowTruckExtended thisExtended = mySQL2.GetExtendedData(thisTruck.Identifier);
                            thisTruck.Extended = thisExtended;
                            thisTruck.TruckNumber = thisExtended.TruckNumber;
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        thisTruck = new TowTruck.TowTruck(ip);
                        DataClasses.GlobalData.AddTowTruck(thisTruck);
                        SQL.SQLCode mySQL2 = new SQL.SQLCode();
                        TowTruck.TowTruckExtended thisExtended = mySQL2.GetExtendedData(thisTruck.Identifier);
                        thisTruck.Extended = thisExtended;
                        thisTruck.TruckNumber = thisExtended.TruckNumber;
                        thisTruck.Status.StatusStarted = DateTime.Now;
                    }
                    catch { }
                }

                thisTruck.UpdateGPS(thisGPS);
                thisTruck.TowTruckGPSUpdate();
            }

            //make sure to clear the alarms when the driver logs on.
            bool NewDriver = false;
            SQL.SQLCode mySQL = new SQL.SQLCode();
            string DataOut = "";

            bool lateLogon = false;

            bool validDriver = mySQL.CheckLogon(ip, FSPIDNumber, Password);
            if (validDriver == true)
            {
                TowTruck.TowTruck thisTruck;
                thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
                if (thisTruck == null)
                {
                    validDriver = false;
                    DataOut = "ERROR You have not been logged on, your truck is not connected";
                }
                else
                {
                    //Make sure driver only logs onto one truck at a time
                    int LogonCount = DataClasses.GlobalData.GetDriverLogonCount(FSPIDNumber, thisTruck.TruckNumber);
                    int BeatCoverCount = DataClasses.GlobalData.GetCoverCount(thisTruck.TruckNumber);
                    if (LogonCount > 0)
                    {
                        validDriver = false;
                        DataOut = "ERROR: You are currently logged on to another vehicle";
                    }
                    else if (BeatCoverCount > 0)
                    {
                        validDriver = false;
                        DataOut = "ERROR: You can only patrol one beat at a time";
                    }
                    else
                    {
                        TowTruck.TowTruckDriver thisDriver = mySQL.GetDriver(FSPIDNumber);
                        thisDriver.FSPID = FSPIDNumber;
                        if (AssignedBeat != thisTruck.assignedBeat.BeatNumber)
                        {
                            //logged onto new beat, clear any alarms
                            ClearTruckAlarms(thisTruck);
                        }
                        DataClasses.GlobalData.AddCover(thisTruck.TruckNumber, AssignedBeat);
                        if (thisTruck.Driver == null || thisTruck.Driver.FSPID != thisDriver.FSPID)
                        {
                            thisTruck.Driver = thisDriver;
                            thisTruck.Driver.FSPID = FSPIDNumber;
                            NewDriver = true;
                        }
                        if (!string.IsNullOrEmpty(AssignedBeat))
                        {
                            BeatData.Beat thisBeat = BeatData.Beats.GetDriverBeat(new Guid(AssignedBeat));
                            if (thisBeat != null)
                            {
                                thisTruck.assignedBeat.BeatID = thisBeat.BeatID;
                                thisTruck.assignedBeat.BeatNumber = thisBeat.BeatNumber;
                                thisTruck.assignedBeat.BeatExtent = thisBeat.BeatExtent;
                                thisTruck.assignedBeat.Loaded = true;
                                thisTruck.thisSchedule = DataClasses.GlobalData.FindAppropriateSchedule(thisBeat.BeatID);
                                if (thisTruck.thisSchedule != null)
                                {
                                    //check for appropridate log on time for beat and leeway for logon time.
                                    DateTime AcceptedLogonTimeStart = thisTruck.thisSchedule.Logon.AddMinutes(DataClasses.GlobalData.LogOnLeeway * -1);
                                    DateTime AcceptedLogonTimeStop = thisTruck.thisSchedule.Logon.AddMinutes(DataClasses.GlobalData.LogOnLeeway);
                                    if (DateTime.Now < AcceptedLogonTimeStart || DateTime.Now > AcceptedLogonTimeStop)
                                    {
                                        thisTruck.Status.LogOnAlarm = true;
                                        thisTruck.Status.LogOnAlarmTime = DateTime.Now;
                                        mySQL.LogAlarm("Logon", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID, thisTruck.assignedBeat.BeatID);
                                    }
                                    if (DateTime.Now > AcceptedLogonTimeStop)
                                    {
                                        //DataOut = "LateLogOn";
                                        lateLogon = true;
                                    }
                                }

                            }
                            else
                            {
                                thisTruck.assignedBeat.BeatID = new Guid("00000000-0000-0000-0000-000000000000");
                                thisTruck.assignedBeat.Loaded = false;
                                thisTruck.thisSchedule = null;
                            }
                        }
                        else
                        {
                            thisTruck.assignedBeat.BeatID = new Guid("00000000-0000-0000-0000-000000000000");
                            thisTruck.assignedBeat.Loaded = false;
                            thisTruck.thisSchedule = null;
                        }
                        string CurrentStatus = thisTruck.Status.VehicleStatus;
                        if (CurrentStatus == "On Lunch" && NewDriver == false)
                        {
                            TimeSpan ts = DateTime.Now - thisTruck.Driver.LunchStarted;
                            int LunchTime = Convert.ToInt32(ts.TotalMinutes);
                            //thisTruck.Driver.LunchStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                            mySQL = new SQL.SQLCode();
                            mySQL.SetBreakTime(thisTruck.Driver.DriverID, "Lunch", LunchTime);
                        }
                        if (CurrentStatus == "On Break" && NewDriver == false)
                        {
                            TimeSpan ts = DateTime.Now - thisTruck.Driver.BreakStarted;
                            int BreakTime = Convert.ToInt32(ts.TotalMinutes);
                            //thisTruck.Driver.BreakStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                            mySQL = new SQL.SQLCode();
                            mySQL.SetBreakTime(thisTruck.Driver.DriverID, "Break", BreakTime);
                        }
                        thisTruck.Status.VehicleStatus = "Driver Logged On";
                        //Add check to set used break or lunch time here.
                        //mySQL.LogEvent(thisDriver.DriverID, "Driver Log On");
                        string beatNumber = thisTruck.assignedBeat.BeatNumber;
                        if (string.IsNullOrEmpty(beatNumber))
                        {
                            beatNumber = "NA";
                        }
                        mySQL.LogStatusChange(thisTruck.Driver.FirstName + " " + thisTruck.Driver.LastName, thisTruck.Extended.TruckNumber, "Log On", DateTime.Now, beatNumber);
                        thisTruck.Status.StatusStarted = DateTime.Now;
                        DataOut = thisDriver.LastName + ", " + thisDriver.FirstName + "|" + thisTruck.TruckNumber + "|" + thisDriver.DriverID + "|" +
                                thisTruck.Extended.FleetVehicleID + "|" + thisTruck.Extended.ContractorID + "|" + thisTruck.assignedBeat.BeatNumber;

                        if (lateLogon == true)
                        {
                            DataOut += "|late";
                        }
                        else
                        {
                            DataOut += "|ontime";
                        }
                    }
                }
            }
            else
            {
                DataOut = "ERROR: Your logon credentials cannot be verified";
            }

            return DataOut;

        }

        [OperationContract]
        [WebGet]
        public string DriverLogoff(string Mac = "0", string _ok = "check")
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }
            }

            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                bool logoffOK = true;
                SQL.SQLCode mySQL = new SQL.SQLCode();

                if (thisTruck.thisSchedule != null)
                {
                    DateTime AcceptedLogOffStart = thisTruck.thisSchedule.LogOff.AddMinutes(DataClasses.GlobalData.LogOffLeeway * -1);
                    DateTime AcceptedLogOffStop = thisTruck.thisSchedule.LogOff.AddMinutes(DataClasses.GlobalData.LogOffLeeway);
                    if (DateTime.Now < AcceptedLogOffStart || DateTime.Now > AcceptedLogOffStop)
                    {
                        thisTruck.Status.LogOffAlarm = true;
                        thisTruck.Status.LogOffAlarmTime = DateTime.Now;
                        mySQL.LogAlarm("LogOff", DateTime.Now, thisTruck.Driver.DriverID, thisTruck.Extended.FleetVehicleID, thisTruck.assignedBeat.BeatID);
                    }
                    if (DateTime.Now < AcceptedLogOffStart)
                    {
                        logoffOK = false;
                    }
                }

                if (_ok == "force")
                {
                    logoffOK = true;
                }

                if (logoffOK == true)
                {
                    //mySQL.LogEvent(thisTruck.Driver.DriverID, "Driver Log Off");
                    string beatNumber = thisTruck.assignedBeat.BeatNumber;
                    if (string.IsNullOrEmpty(beatNumber))
                    {
                        beatNumber = "NA";
                    }
                    mySQL.LogStatusChange(thisTruck.Driver.FirstName + " " + thisTruck.Driver.LastName, thisTruck.Extended.TruckNumber, "Log Off", DateTime.Now, beatNumber);
                    DataClasses.GlobalData.RemoveCover(thisTruck.TruckNumber);
                    thisTruck.Status.VehicleStatus = "Waiting for Driver Login";
                    thisTruck.Driver.DriverID = new Guid("00000000-0000-0000-0000-000000000000");
                    thisTruck.Driver.FSPID = "";
                    thisTruck.Driver.FirstName = "No Driver";
                    thisTruck.Driver.LastName = "No Driver";
                    thisTruck.Driver.TowTruckCompany = "No Driver";
                    //thisTruck.Driver.BreakStarted = Convert.ToDateTime("01/01/2001 00:00:00");
                    thisTruck.assignedBeat.BeatID = new Guid("00000000-0000-0000-0000-000000000000");
                    thisTruck.assignedBeat.BeatExtent = null;
                    thisTruck.assignedBeat.BeatNumber = "Not Assigned";
                    thisTruck.assignedBeat.Loaded = false;
                    thisTruck.Status.SpeedingTime = Convert.ToDateTime("01/01/2001 00:00:00");
                    thisTruck.Status.OutOfBoundsTime = Convert.ToDateTime("01/01/2001 00:00:00");
                    thisTruck.Status.SpeedingValue = 0.0;
                    thisTruck.Status.OutOfBoundsMessage = "Not logged on";
                    thisTruck.Status.OutOfBoundsAlarm = false;
                    thisTruck.Status.SpeedingAlarm = false;
                    thisTruck.Status.StatusStarted = DateTime.Now;
                    thisTruck.thisSchedule = null;
                    return "OK";
                }
                else
                {
                    return "early";
                }
            }
            else
            {
                return "notruck";
            }
        }

        [OperationContract]
        [WebGet]
        public int GetBreakDuration(string DriverID)
        {
            //Guid gDriverID = new Guid(DriverID);
            SQL.SQLCode mySQL = new SQL.SQLCode();
            int BreakDuration = mySQL.GetBreakDuration(DriverID);
            return BreakDuration;
        }

        [OperationContract]
        [WebGet]
        public int GetLunchDuration(string DriverID)
        {
            int LunchDuration = 30;
            return LunchDuration;
        }

        [OperationContract]
        [WebGet]
        public int FindUsedBreakTime(string DriverID, string Type)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            return mySQL.GetUsedBreakTime(DriverID, Type);
        }

        //Deprecated, this was used for a count down method, FindUsedBreakTime pulls from database and is used to count upwards
        [OperationContract]
        [WebGet]
        public int GetUsedBreakTime(string DriverID)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            int diffTime = 0;
            if (thisTruck != null)
            {
                DateTime breakStarted = thisTruck.Driver.BreakStarted;
                if (breakStarted == Convert.ToDateTime("01/01/2001 00:00:00"))
                {
                    breakStarted = DateTime.Now;
                    thisTruck.Driver.BreakStarted = breakStarted;
                }
                TimeSpan ts = DateTime.Now.Subtract(breakStarted);
                diffTime = Convert.ToInt32(ts.TotalMinutes);
                SQL.SQLCode mySQL = new SQL.SQLCode();
                int BreakDuration = mySQL.GetBreakDuration(DriverID);
                diffTime = BreakDuration - diffTime;
            }
            return diffTime;
        }

        //Deprecated, this was used for a count down method, FindUsedBreakTime pulls from database and is used to count upwards
        [OperationContract]
        [WebGet]
        public int GetUsedLunchTime(string DriverID)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            int diffTime = 0;
            if (thisTruck != null)
            {
                DateTime lunchStarted = thisTruck.Driver.LunchStarted;
                if (lunchStarted == Convert.ToDateTime("01/01/2001 00:00:00"))
                {
                    lunchStarted = DateTime.Now;
                    thisTruck.Driver.LunchStarted = lunchStarted;
                }
                TimeSpan ts = DateTime.Now.Subtract(lunchStarted);
                diffTime = Convert.ToInt32(ts.TotalMinutes);
                //SQL.SQLCode mySQL = new SQL.SQLCode();
                int LunchDuration = 30;
                diffTime = LunchDuration - diffTime;
            }
            return diffTime;
        }

        private void ClearTruckAlarms(TowTruck.TowTruck t)
        {
            t.Status.LogOnAlarm = false;
            t.Status.LogOnAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOnAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOnAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOnAlarmComments = "NA";
            t.Status.RollOutAlarm = false;
            t.Status.RollInAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmComments = "NA";
            t.Status.OnPatrolAlarm = false;
            t.Status.OnPatrolAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.OnPatrolAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.OnPatrolAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.OnPatrolAlarmComments = "NA";
            t.Status.RollInAlarm = false;
            t.Status.RollInAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.RollInAlarmComments = "NA";
            t.Status.LogOffAlarm = false;
            t.Status.LogOffAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOffAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOffAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.LogOffAlarmComments = "NA";
            t.Status.GPSIssueAlarm = false;
            t.Status.GPSIssueAlarmStart = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.GPSIssueAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.GPSIssueAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.GPSIssueAlarmComments = "NA";
            t.Status.IncidentAlarm = false;
            t.Status.IncidentAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.IncidentAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.IncidentAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.IncidentAlarmComments = "NA";
            t.Status.StationaryAlarm = false;
            t.Status.StationaryAlarmStart = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.StationaryAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.StationaryAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            t.Status.StationaryAlarmComments = "NA";
        }

        #endregion

        #region " find preloaded data for client tablet "

        [OperationContract]
        [WebGet]
        public string GetBeats()
        {
            List<MiscData.ClientBeat> myBeats = new List<MiscData.ClientBeat>();
            myBeats.Add(new MiscData.ClientBeat
            {
                BeatName = "000-000",
                BeatID = new Guid("00000000-0000-0000-0000-000000000000")
            });
            foreach (BeatData.Beat thisBeat in BeatData.Beats.AllBeats)
            {
                MiscData.ClientBeat thisClient = new MiscData.ClientBeat();
                thisClient.BeatID = thisBeat.BeatID;
                thisClient.BeatName = thisBeat.BeatNumber;
                myBeats.Add(thisClient);
            }
            myBeats = myBeats.OrderBy(x => x.BeatName).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(myBeats);
        }

        [OperationContract]
        [WebGet]
        public string GetPreloadedData(string Type)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string JSONData = "";
            switch (Type)
            {
                case "Code1098s":
                    List<DataClasses.ClientCode1098> Code1098s = new List<DataClasses.ClientCode1098>();
                    foreach (MiscData.Code1098 thisCode in DataClasses.GlobalData.Code1098s)
                    {
                        Code1098s.Add(new DataClasses.ClientCode1098
                        {
                            CodeID = thisCode.CodeID,
                            CodeCall = thisCode.CodeCall
                        });
                    }
                    JSONData = js.Serialize(Code1098s);
                    break;
                case "Freeways":
                    List<DataClasses.ClientFreeway> Freeways = new List<DataClasses.ClientFreeway>();
                    foreach (MiscData.Freeway thisFreeway in DataClasses.GlobalData.Freeways)
                    {
                        Freeways.Add(new DataClasses.ClientFreeway
                        {
                            FreewayID = thisFreeway.FreewayID,
                            FreewayName = thisFreeway.FreewayName
                        });
                    }
                    JSONData = js.Serialize(Freeways);
                    break;
                case "IncidentTypes":
                    List<DataClasses.ClientIncidentType> IncidentTypes = new List<DataClasses.ClientIncidentType>();
                    foreach (MiscData.IncidentType thisIncident in DataClasses.GlobalData.IncidentTypes)
                    {
                        IncidentTypes.Add(new DataClasses.ClientIncidentType
                        {
                            IncidentTypeID = thisIncident.IncidentTypeID,
                            IncidentTypeCode = thisIncident.IncidentTypeCode,
                            IncidentTypeName = thisIncident.IncidentTypeName
                        });
                    }
                    JSONData = js.Serialize(IncidentTypes);
                    break;
                case "LocationCodes":
                    List<DataClasses.ClientLocationCode> LocationCodes = new List<DataClasses.ClientLocationCode>();
                    foreach (MiscData.LocationCoding thisCode in DataClasses.GlobalData.LocationCodes)
                    {
                        LocationCodes.Add(new DataClasses.ClientLocationCode
                        {
                            LocationID = thisCode.LocationID,
                            LocationCode = thisCode.LocationCode
                        });
                    }
                    JSONData = js.Serialize(LocationCodes);
                    break;
                case "ServiceTypes":
                    List<DataClasses.ClientServiceType> ServiceTypes = new List<DataClasses.ClientServiceType>();
                    foreach (MiscData.ServiceType thisServiceType in DataClasses.GlobalData.ServiceTypes)
                    {
                        ServiceTypes.Add(new DataClasses.ClientServiceType
                        {
                            ServiceTypeID = thisServiceType.ServiceTypeID,
                            ServiceTypeCode = thisServiceType.ServiceTypeCode,
                            ServiceTypeName = thisServiceType.ServiceTypeName
                        });
                    }
                    JSONData = js.Serialize(ServiceTypes);
                    break;
                case "TowLocations":
                    List<DataClasses.ClientTowLocation> TowLocations = new List<DataClasses.ClientTowLocation>();
                    foreach (MiscData.TowLocation thisTowLocation in DataClasses.GlobalData.TowLocations)
                    {
                        TowLocations.Add(new DataClasses.ClientTowLocation
                        {
                            TowLocationID = thisTowLocation.TowLocationID,
                            TowLocationCode = thisTowLocation.TowLocationCode,
                            TowLocationName = thisTowLocation.TowLocationName
                        });
                    }
                    JSONData = js.Serialize(TowLocations);
                    break;
                case "TrafficSpeeds":
                    List<DataClasses.ClientTrafficSpeed> TrafficSpeeds = new List<DataClasses.ClientTrafficSpeed>();
                    foreach (MiscData.TrafficSpeed thisSpeed in DataClasses.GlobalData.TrafficSpeeds)
                    {
                        TrafficSpeeds.Add(new DataClasses.ClientTrafficSpeed
                        {
                            TrafficSpeedID = thisSpeed.TrafficSpeedID,
                            TrafficSpeedCode = thisSpeed.TrafficSpeedCode
                        });
                    }
                    JSONData = js.Serialize(TrafficSpeeds);
                    break;
                case "VehiclePositions":
                    List<DataClasses.ClientVehiclePosition> VehiclePositions = new List<DataClasses.ClientVehiclePosition>();
                    foreach (MiscData.VehiclePosition thisPosition in DataClasses.GlobalData.VehiclePositions)
                    {
                        VehiclePositions.Add(new DataClasses.ClientVehiclePosition
                        {
                            VehiclePositionID = thisPosition.VehiclePositionID,
                            VehiclePositionCode = thisPosition.VehiclePositionCode
                        });
                    }
                    JSONData = js.Serialize(VehiclePositions);
                    break;
                case "VehicleTypes":
                    List<DataClasses.ClientVehicleTypes> VehicleTypes = new List<DataClasses.ClientVehicleTypes>();
                    foreach (MiscData.VehicleType thisType in DataClasses.GlobalData.VehicleTypes)
                    {
                        VehicleTypes.Add(new DataClasses.ClientVehicleTypes
                        {
                            VehicleTypeID = thisType.VehicleTypeID,
                            VehicleTypeCode = thisType.VehicleTypeCode
                        });
                    }
                    JSONData = js.Serialize(VehicleTypes);
                    break;
                case "DropZones":
                    List<DataClasses.ClientDropZone> DropZones = new List<DataClasses.ClientDropZone>();
                    foreach (MiscData.DropZone thisZone in DataClasses.GlobalData.DropZones)
                    {
                        DropZones.Add(new DataClasses.ClientDropZone
                        {
                            DropZoneID = thisZone.DropZoneID,
                            Location = thisZone.Location
                        });
                    }
                    JSONData = js.Serialize(DropZones);
                    break;
                case "Contractors":
                    List<DataClasses.ClientContractor> Contractors = new List<DataClasses.ClientContractor>();
                    foreach (MiscData.Contractors thisContractor in DataClasses.GlobalData.Contractors)
                    {
                        Contractors.Add(new DataClasses.ClientContractor
                        {
                            ContractorID = thisContractor.ContractorID,
                            ContractCompanyName = thisContractor.ContractCompanyName
                        });
                    }
                    JSONData = js.Serialize(Contractors);
                    break;
                default:
                    JSONData = "NO DATA";
                    break;
            }
            return JSONData;
        }

        #endregion

        #region " Truck MEssages (Not assist) "

        [OperationContract]
        [WebGet]
        public string GetTruckMessages(string Mac = "0")
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }

            }
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ip);
            if (thisTruck != null)
            {
                List<TruckMessage> myMessages = DataClasses.GlobalData.GetMessagesByTruck(thisTruck.Identifier);
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(myMessages);
            }
            else
            {
                return "";
            }
        }

        [OperationContract]
        [WebGet]
        public void AckMessage(string MessageID)
        {
            Guid msgID = new Guid(MessageID);
            DataClasses.GlobalData.AckTruckMessage(msgID);
        }

        #endregion

        #region "Driver Loggiong"

        [OperationContract]
        [WebInvoke]
        public void PostDriverAppLogEntry(string driverName, string truckNumber, string title, string details, string Mac)
        {
            string ip = "";
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ip = endpoint.Address;
            //temp fix, endpoint.Address is returning IPv6 loopback address when tested locally
            if (ip == "::1")
            { ip = "127.0.0.1"; }

            if (Mac != null)
            {
                if (Mac != "0")
                {
                    ip = Mac.ToUpper();
                }
            }

            SQL.SQLCode mySQL = new SQL.SQLCode();
            mySQL.LogDriverEntry(driverName, truckNumber, title, details, Mac);
        }
        #endregion
    }
}
