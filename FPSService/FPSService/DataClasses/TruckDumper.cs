using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using System.Configuration;
using System.Web.Script.Serialization;
using System.ServiceModel;

namespace FPSService.DataClasses
{
    //This sends current truck data to a secondary server or servers for demo and testing purposes
    public class TruckDumper
    {
        Logging.MessageLogger logger = new Logging.MessageLogger("TowTruckDumper.txt");
        Logging.EventLogger evtLog;
        Timer dumpTimer;

        public TruckDumper()
        {
            string forward = ConfigurationManager.AppSettings["forward"].ToString();
            if (forward.ToUpper() != "TRUE")
            {
                return;
            }
            else
            {
                dumpTimer = new Timer(30000);
                dumpTimer.Elapsed += new ElapsedEventHandler(dumpTimer_Elapsed);
                dumpTimer.Start();
            }
        }

        void dumpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                if (DataClasses.GlobalData.currentTrucks.Count > 0)
                {
                    Guid _beatID = new Guid("00000000-0000-0000-0000-000000000000");
                    string[] srvListString = ConfigurationManager.AppSettings["OtherServers"].ToString().Split('|');
                    //List<TowTruck.TowTruck> allTrucks = new List<TowTruck.TowTruck>();
                    List<srSecondaryService.CopyTruck> ctList = new List<srSecondaryService.CopyTruck>();
                    foreach (TowTruck.TowTruck t in DataClasses.GlobalData.currentTrucks)
                    {
                        if (t.assignedBeat.BeatID != null && t.assignedBeat.BeatID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            _beatID = t.GPSPosition.BeatID;
                        }
                        srSecondaryService.CopyTruck newTruck = new srSecondaryService.CopyTruck();
                        newTruck.Status = new srSecondaryService.CopyStatus();
                        newTruck.Driver = new srSecondaryService.CopyDriver();
                        newTruck.Extended = new srSecondaryService.CopyExtended();
                        newTruck.Identifier = t.Identifier;
                        newTruck.BeatID = t.assignedBeat.BeatID;
                        if (!string.IsNullOrEmpty(t.assignedBeat.BeatNumber))
                        {
                            newTruck.BeatNumber = t.assignedBeat.BeatNumber;
                        }
                        else
                        {
                            newTruck.BeatNumber = "UNASSIGNED";
                        }
                        //set up Status
                        newTruck.Status.SpeedingAlarm = t.Status.SpeedingAlarm;
                        newTruck.Status.SpeedingValue = t.Status.SpeedingValue;
                        newTruck.Status.SpeedingTime = t.Status.SpeedingTime;
                        newTruck.Status.OutOfBoundsAlarm = t.Status.OutOfBoundsAlarm;
                        newTruck.Status.OutOfBoundsMessage = t.Status.OutOfBoundsMessage;
                        newTruck.Status.OutOfBoundsTime = t.Status.OutOfBoundsTime;
                        newTruck.Status.OutOfBoundsStartTime = t.Status.OutOfBoundsStartTime;
                        newTruck.Status.VehicleStatus = t.Status.VehicleStatus;
                        newTruck.Status.StatusStarted = t.Status.StatusStarted;
                        newTruck.Status.LogOnAlarm = t.Status.LogOnAlarm;
                        newTruck.Status.LogOnAlarmTime = t.Status.LogOnAlarmTime;
                        newTruck.Status.LogOnAlarmCleared = t.Status.LogOnAlarmCleared;
                        newTruck.Status.LogOnAlarmExcused = t.Status.LogOnAlarmExcused;
                        newTruck.Status.LogOnAlarmComments = t.Status.LogOnAlarmComments;
                        newTruck.Status.RollOutAlarm = t.Status.RollOutAlarm;
                        newTruck.Status.RollOutAlarmTime = t.Status.RollOutAlarmTime;
                        newTruck.Status.RollOutAlarmCleared = t.Status.RollOutAlarmCleared;
                        newTruck.Status.RollOutAlarmExcused = t.Status.RollOutAlarmExcused;
                        newTruck.Status.RollOutAlarmComments = t.Status.RollOutAlarmComments;
                        newTruck.Status.OnPatrolAlarm = t.Status.OnPatrolAlarm;
                        newTruck.Status.OnPatrolAlarmTime = t.Status.OnPatrolAlarmTime;
                        newTruck.Status.OnPatrolAlarmCleared = t.Status.OnPatrolAlarmCleared;
                        newTruck.Status.OnPatrolAlarmExcused = t.Status.OnPatrolAlarmExcused;
                        newTruck.Status.OnPatrolAlarmComments = t.Status.OnPatrolAlarmComments;
                        newTruck.Status.RollInAlarm = t.Status.RollInAlarm;
                        newTruck.Status.RollInAlarmTime = t.Status.RollInAlarmTime;
                        newTruck.Status.RollInAlarmCleared = t.Status.RollInAlarmCleared;
                        newTruck.Status.RollInAlarmExcused = t.Status.RollInAlarmExcused;
                        newTruck.Status.RollInAlarmComments = t.Status.RollInAlarmComments;
                        newTruck.Status.LogOffAlarm = t.Status.LogOffAlarm;
                        newTruck.Status.LogOffAlarmTime = t.Status.LogOffAlarmTime;
                        newTruck.Status.LogOffAlarmCleared = t.Status.LogOffAlarmCleared;
                        newTruck.Status.LogOffAlarmExcused = t.Status.LogOffAlarmExcused;
                        newTruck.Status.LogOffAlarmComments = t.Status.LogOffAlarmComments;
                        newTruck.Status.IncidentAlarm = t.Status.IncidentAlarm;
                        newTruck.Status.IncidentAlarmTime = t.Status.IncidentAlarmTime;
                        newTruck.Status.IncidentAlarmCleared = t.Status.IncidentAlarmCleared;
                        newTruck.Status.IncidentAlarmExcused = t.Status.IncidentAlarmExcused;
                        newTruck.Status.IncidentAlarmComments = t.Status.IncidentAlarmComments;
                        newTruck.Status.GPSIssueAlarm = t.Status.GPSIssueAlarm;  //handles NO GPS
                        newTruck.Status.GPSIssueAlarmStart = t.Status.GPSIssueAlarmStart; //handles NO GPS
                        newTruck.Status.GPSIssueAlarmCleared = t.Status.GPSIssueAlarmCleared;
                        newTruck.Status.GPSIssueAlarmExcused = t.Status.GPSIssueAlarmExcused;
                        newTruck.Status.GPSIssueAlarmComments = t.Status.GPSIssueAlarmComments;
                        newTruck.Status.StationaryAlarm = t.Status.StationaryAlarm; //handles no movement, speed = 0
                        newTruck.Status.StationaryAlarmStart = t.Status.StationaryAlarmStart; //handles no movement, speed = 0
                        newTruck.Status.StationaryAlarmCleared = t.Status.StationaryAlarmCleared;
                        newTruck.Status.StationaryAlarmExcused = t.Status.StationaryAlarmExcused;
                        newTruck.Status.StationaryAlarmComments = t.Status.StationaryAlarmComments;
                        //update driver
                        newTruck.Driver.DriverID = t.Driver.DriverID;
                        newTruck.Driver.LastName = t.Driver.LastName;
                        newTruck.Driver.FirstName = t.Driver.FirstName;
                        newTruck.Driver.TowTruckCompany = t.Driver.TowTruckCompany;
                        newTruck.Driver.FSPID = t.Driver.FSPID;
                        newTruck.Driver.AssignedBeat = t.Driver.AssignedBeat;
                        newTruck.Driver.BeatScheduleID = t.Driver.BeatScheduleID;
                        newTruck.Driver.BreakStarted = t.Driver.BreakStarted;
                        newTruck.Driver.LunchStarted = t.Driver.LunchStarted;
                        //Extended data
                        newTruck.Extended.ContractorName = t.Extended.ContractorName;
                        newTruck.Extended.ContractorID = t.Extended.ContractorID;
                        newTruck.Extended.TruckNumber = t.Extended.TruckNumber;
                        newTruck.Extended.FleetNumber = t.Extended.FleetNumber;
                        newTruck.Extended.ProgramStartDate = t.Extended.ProgramStartDate;
                        newTruck.Extended.VehicleType = t.Extended.VehicleType;
                        newTruck.Extended.VehicleYear = t.Extended.VehicleYear;
                        newTruck.Extended.VehicleMake = t.Extended.VehicleMake;
                        newTruck.Extended.VehicleModel = t.Extended.VehicleModel;
                        newTruck.Extended.LicensePlate = t.Extended.LicensePlate;
                        newTruck.Extended.RegistrationExpireDate = t.Extended.RegistrationExpireDate;
                        newTruck.Extended.InsuranceExpireDate = t.Extended.InsuranceExpireDate;
                        newTruck.Extended.LastCHPInspection = t.Extended.LastCHPInspection;
                        newTruck.Extended.ProgramEndDate = t.Extended.ProgramEndDate;
                        newTruck.Extended.FAW = t.Extended.FAW;
                        newTruck.Extended.RAW = t.Extended.RAW;
                        newTruck.Extended.RAWR = t.Extended.RAWR;
                        newTruck.Extended.GVW = t.Extended.GVW;
                        newTruck.Extended.GVWR = t.Extended.GVWR;
                        newTruck.Extended.Wheelbase = t.Extended.Wheelbase;
                        newTruck.Extended.Overhang = t.Extended.Overhang;
                        newTruck.Extended.MAXTW = t.Extended.MAXTW;
                        newTruck.Extended.MAXTWCALCDATE = t.Extended.MAXTWCALCDATE;
                        newTruck.Extended.FuelType = t.Extended.FuelType;
                        newTruck.Extended.FleetVehicleID = t.Extended.FleetVehicleID;
                        ctList.Add(newTruck);
                        
                    }

                    for (int i = 0; i < srvListString.Count(); i++)
                    {
                        srSecondaryService.TowTruckServiceClient sr = new srSecondaryService.TowTruckServiceClient();
                        sr.Endpoint.Address = new EndpointAddress(new Uri("http://" + srvListString[i].ToString() + ":9007/TowTruckService.svc"));
                        foreach (srSecondaryService.CopyTruck t in ctList)
                        {
                            sr.SingleTruckDump(t);
                        }
                        //sr.TruckDump(ctList.ToArray());
                    }
                }
            }
            catch(Exception ex)
            {
                evtLog = new Logging.EventLogger();
                evtLog.LogEvent("Error dumping trucks to secondary service" + Environment.NewLine + ex.ToString(), true);
                logger.writeToLogFile(DateTime.Now.ToString() + Environment.NewLine + "Error dumping trucks to secondary service" + Environment.NewLine + ex.ToString() + Environment.NewLine +
                    Environment.NewLine);
            }
        }
    }
}