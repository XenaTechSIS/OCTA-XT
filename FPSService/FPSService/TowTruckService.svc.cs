using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using FPSService.MiscData;

namespace FPSService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TowTruckService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(
     RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TowTruckService : ITowTruckService
    {
        private Logging.EventLogger logger; //error logging

        public void UpdateTruckNumber(string IPAddress, string TruckNumber, Guid ContractorID)
        {

        }

        public void SingleTruckDump(CopyTruck t)
        {
            try
            {
                TowTruck.TowTruck fTruck = DataClasses.GlobalData.currentTrucks.Find(delegate (TowTruck.TowTruck dTruck) { return dTruck.Identifier == t.Identifier; });
                if (fTruck == null)
                {
                    //if no data is being forwarded, the truck could be destroyed by the target service, we don't
                    //want to recreate one with null gps data.
                    /*
                    TowTruck.TowTruck newTruck = new TowTruck.TowTruck(t.Identifier);
                    newTruck.GPSPosition.Lat = 0;
                    newTruck.GPSPosition.Lon = 0;
                    DataClasses.GlobalData.currentTrucks.Add(newTruck);
                     * */
                }
                else
                {
                    //update truck status
                    fTruck.Status = new TowTruck.TowTruckStatus();
                    fTruck.Status.SpeedingAlarm = t.Status.SpeedingAlarm;
                    fTruck.Status.SpeedingValue = t.Status.SpeedingValue;
                    fTruck.Status.SpeedingTime = t.Status.SpeedingTime;
                    fTruck.assignedBeat.BeatNumber = t.BeatNumber;
                    fTruck.assignedBeat.BeatID = t.BeatID;
                    //fTruck.Status.SpeedingLocation = t.Status.SpeedingLocation;
                    fTruck.Status.OutOfBoundsAlarm = t.Status.OutOfBoundsAlarm;
                    fTruck.Status.OutOfBoundsMessage = t.Status.OutOfBoundsMessage;
                    fTruck.Status.OutOfBoundsTime = t.Status.OutOfBoundsTime;
                    fTruck.Status.OutOfBoundsStartTime = t.Status.OutOfBoundsStartTime;
                    fTruck.Status.VehicleStatus = t.Status.VehicleStatus;
                    fTruck.Status.StatusStarted = t.Status.StatusStarted;
                    fTruck.Status.LogOnAlarm = t.Status.LogOnAlarm;
                    fTruck.Status.LogOnAlarmTime = t.Status.LogOnAlarmTime;
                    fTruck.Status.LogOnAlarmCleared = t.Status.LogOnAlarmCleared;
                    fTruck.Status.LogOnAlarmExcused = t.Status.LogOnAlarmExcused;
                    fTruck.Status.LogOnAlarmComments = t.Status.LogOnAlarmComments;
                    fTruck.Status.RollOutAlarm = t.Status.RollOutAlarm;
                    fTruck.Status.RollOutAlarmTime = t.Status.RollOutAlarmTime;
                    fTruck.Status.RollOutAlarmCleared = t.Status.RollOutAlarmCleared;
                    fTruck.Status.RollOutAlarmExcused = t.Status.RollOutAlarmExcused;
                    fTruck.Status.RollOutAlarmComments = t.Status.RollOutAlarmComments;
                    fTruck.Status.OnPatrolAlarm = t.Status.OnPatrolAlarm;
                    fTruck.Status.OnPatrolAlarmTime = t.Status.OnPatrolAlarmTime;
                    fTruck.Status.OnPatrolAlarmCleared = t.Status.OnPatrolAlarmCleared;
                    fTruck.Status.OnPatrolAlarmExcused = t.Status.OnPatrolAlarmExcused;
                    fTruck.Status.OnPatrolAlarmComments = t.Status.OnPatrolAlarmComments;
                    fTruck.Status.RollInAlarm = t.Status.RollInAlarm;
                    fTruck.Status.RollInAlarmTime = t.Status.RollInAlarmTime;
                    fTruck.Status.RollInAlarmCleared = t.Status.RollInAlarmCleared;
                    fTruck.Status.RollInAlarmExcused = t.Status.RollInAlarmExcused;
                    fTruck.Status.RollInAlarmComments = t.Status.RollInAlarmComments;
                    fTruck.Status.LogOffAlarm = t.Status.LogOffAlarm;
                    fTruck.Status.LogOffAlarmTime = t.Status.LogOffAlarmTime;
                    fTruck.Status.LogOffAlarmCleared = t.Status.LogOffAlarmCleared;
                    fTruck.Status.LogOffAlarmExcused = t.Status.LogOffAlarmExcused;
                    fTruck.Status.LogOffAlarmComments = t.Status.LogOffAlarmComments;
                    fTruck.Status.IncidentAlarm = t.Status.IncidentAlarm;
                    fTruck.Status.IncidentAlarmTime = t.Status.IncidentAlarmTime;
                    fTruck.Status.IncidentAlarmCleared = t.Status.IncidentAlarmCleared;
                    fTruck.Status.IncidentAlarmExcused = t.Status.IncidentAlarmExcused;
                    fTruck.Status.IncidentAlarmComments = t.Status.IncidentAlarmComments;
                    fTruck.Status.GPSIssueAlarm = t.Status.GPSIssueAlarm;  //handles NO GPS
                    fTruck.Status.GPSIssueAlarmStart = t.Status.GPSIssueAlarmStart; //handles NO GPS
                    fTruck.Status.GPSIssueAlarmCleared = t.Status.GPSIssueAlarmCleared;
                    fTruck.Status.GPSIssueAlarmExcused = t.Status.GPSIssueAlarmExcused;
                    fTruck.Status.GPSIssueAlarmComments = t.Status.GPSIssueAlarmComments;
                    fTruck.Status.StationaryAlarm = t.Status.StationaryAlarm; //handles no movement, speed = 0
                    fTruck.Status.StationaryAlarmStart = t.Status.StationaryAlarmStart; //handles no movement, speed = 0
                    fTruck.Status.StationaryAlarmCleared = t.Status.StationaryAlarmCleared;
                    fTruck.Status.StationaryAlarmExcused = t.Status.StationaryAlarmExcused;
                    fTruck.Status.StationaryAlarmComments = t.Status.StationaryAlarmComments;
                    //update driver
                    fTruck.Driver.DriverID = t.Driver.DriverID;
                    fTruck.Driver.LastName = t.Driver.LastName;
                    fTruck.Driver.FirstName = t.Driver.FirstName;
                    fTruck.Driver.TowTruckCompany = t.Driver.TowTruckCompany;
                    fTruck.Driver.FSPID = t.Driver.FSPID;
                    fTruck.Driver.AssignedBeat = t.Driver.AssignedBeat;
                    fTruck.Driver.BeatScheduleID = t.Driver.BeatScheduleID;
                    fTruck.Driver.BreakStarted = t.Driver.BreakStarted;
                    fTruck.Driver.LunchStarted = t.Driver.LunchStarted;
                    //Extended data
                    fTruck.Extended.ContractorName = t.Extended.ContractorName;
                    fTruck.Extended.ContractorID = t.Extended.ContractorID;
                    fTruck.Extended.TruckNumber = t.Extended.TruckNumber;
                    fTruck.Extended.FleetNumber = t.Extended.FleetNumber;
                    fTruck.Extended.ProgramStartDate = t.Extended.ProgramStartDate;
                    fTruck.Extended.VehicleType = t.Extended.VehicleType;
                    fTruck.Extended.VehicleYear = t.Extended.VehicleYear;
                    fTruck.Extended.VehicleMake = t.Extended.VehicleMake;
                    fTruck.Extended.VehicleModel = t.Extended.VehicleModel;
                    fTruck.Extended.LicensePlate = t.Extended.LicensePlate;
                    fTruck.Extended.RegistrationExpireDate = t.Extended.RegistrationExpireDate;
                    fTruck.Extended.InsuranceExpireDate = t.Extended.InsuranceExpireDate;
                    fTruck.Extended.LastCHPInspection = t.Extended.LastCHPInspection;
                    fTruck.Extended.ProgramEndDate = t.Extended.ProgramEndDate;
                    fTruck.Extended.FAW = t.Extended.FAW;
                    fTruck.Extended.RAW = t.Extended.RAW;
                    fTruck.Extended.RAWR = t.Extended.RAWR;
                    fTruck.Extended.GVW = t.Extended.GVW;
                    fTruck.Extended.GVWR = t.Extended.GVWR;
                    fTruck.Extended.Wheelbase = t.Extended.Wheelbase;
                    fTruck.Extended.Overhang = t.Extended.Overhang;
                    fTruck.Extended.MAXTW = t.Extended.MAXTW;
                    fTruck.Extended.MAXTWCALCDATE = t.Extended.MAXTWCALCDATE;
                    fTruck.Extended.FuelType = t.Extended.FuelType;
                    fTruck.Extended.FleetVehicleID = t.Extended.FleetVehicleID;
                }
            }
            catch (Exception ex)
            {
                logger = new Logging.EventLogger();
                logger.LogEvent("Error receiving single truck dump" + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void TruckDump(List<CopyTruck> trucks)
        {
            try
            {
                foreach (CopyTruck t in trucks)
                {
                    TowTruck.TowTruck fTruck = DataClasses.GlobalData.currentTrucks.Find(delegate (TowTruck.TowTruck truck) { return truck.Identifier == t.Identifier; });
                    if (fTruck == null)
                    {
                        //if no data is being forwarded, the truck could be destroyed by the target service, we don't
                        //want to recreate one with null gps data.
                        /*
                        TowTruck.TowTruck newTruck = new TowTruck.TowTruck(t.Identifier);
                        newTruck.GPSPosition.Lat = 0;
                        newTruck.GPSPosition.Lon = 0;
                        DataClasses.GlobalData.currentTrucks.Add(newTruck);
                         * */
                    }
                    else
                    {
                        //update truck status
                        fTruck.Status = new TowTruck.TowTruckStatus();
                        fTruck.Status.SpeedingAlarm = t.Status.SpeedingAlarm;
                        fTruck.Status.SpeedingValue = t.Status.SpeedingValue;
                        fTruck.Status.SpeedingTime = t.Status.SpeedingTime;
                        fTruck.assignedBeat.BeatNumber = t.BeatNumber;
                        fTruck.assignedBeat.BeatID = t.BeatID;
                        //fTruck.Status.SpeedingLocation = t.Status.SpeedingLocation;
                        fTruck.Status.OutOfBoundsAlarm = t.Status.OutOfBoundsAlarm;
                        fTruck.Status.OutOfBoundsMessage = t.Status.OutOfBoundsMessage;
                        fTruck.Status.OutOfBoundsTime = t.Status.OutOfBoundsTime;
                        fTruck.Status.OutOfBoundsStartTime = t.Status.OutOfBoundsStartTime;
                        fTruck.Status.VehicleStatus = t.Status.VehicleStatus;
                        fTruck.Status.StatusStarted = t.Status.StatusStarted;
                        fTruck.Status.LogOnAlarm = t.Status.LogOnAlarm;
                        fTruck.Status.LogOnAlarmTime = t.Status.LogOnAlarmTime;
                        fTruck.Status.LogOnAlarmCleared = t.Status.LogOnAlarmCleared;
                        fTruck.Status.LogOnAlarmExcused = t.Status.LogOnAlarmExcused;
                        fTruck.Status.LogOnAlarmComments = t.Status.LogOnAlarmComments;
                        fTruck.Status.RollOutAlarm = t.Status.RollOutAlarm;
                        fTruck.Status.RollOutAlarmTime = t.Status.RollOutAlarmTime;
                        fTruck.Status.RollOutAlarmCleared = t.Status.RollOutAlarmCleared;
                        fTruck.Status.RollOutAlarmExcused = t.Status.RollOutAlarmExcused;
                        fTruck.Status.RollOutAlarmComments = t.Status.RollOutAlarmComments;
                        fTruck.Status.OnPatrolAlarm = t.Status.OnPatrolAlarm;
                        fTruck.Status.OnPatrolAlarmTime = t.Status.OnPatrolAlarmTime;
                        fTruck.Status.OnPatrolAlarmCleared = t.Status.OnPatrolAlarmCleared;
                        fTruck.Status.OnPatrolAlarmExcused = t.Status.OnPatrolAlarmExcused;
                        fTruck.Status.OnPatrolAlarmComments = t.Status.OnPatrolAlarmComments;
                        fTruck.Status.RollInAlarm = t.Status.RollInAlarm;
                        fTruck.Status.RollInAlarmTime = t.Status.RollInAlarmTime;
                        fTruck.Status.RollInAlarmCleared = t.Status.RollInAlarmCleared;
                        fTruck.Status.RollInAlarmExcused = t.Status.RollInAlarmExcused;
                        fTruck.Status.RollInAlarmComments = t.Status.RollInAlarmComments;
                        fTruck.Status.LogOffAlarm = t.Status.LogOffAlarm;
                        fTruck.Status.LogOffAlarmTime = t.Status.LogOffAlarmTime;
                        fTruck.Status.LogOffAlarmCleared = t.Status.LogOffAlarmCleared;
                        fTruck.Status.LogOffAlarmExcused = t.Status.LogOffAlarmExcused;
                        fTruck.Status.LogOffAlarmComments = t.Status.LogOffAlarmComments;
                        fTruck.Status.IncidentAlarm = t.Status.IncidentAlarm;
                        fTruck.Status.IncidentAlarmTime = t.Status.IncidentAlarmTime;
                        fTruck.Status.IncidentAlarmCleared = t.Status.IncidentAlarmCleared;
                        fTruck.Status.IncidentAlarmExcused = t.Status.IncidentAlarmExcused;
                        fTruck.Status.IncidentAlarmComments = t.Status.IncidentAlarmComments;
                        fTruck.Status.GPSIssueAlarm = t.Status.GPSIssueAlarm;  //handles NO GPS
                        fTruck.Status.GPSIssueAlarmStart = t.Status.GPSIssueAlarmStart; //handles NO GPS
                        fTruck.Status.GPSIssueAlarmCleared = t.Status.GPSIssueAlarmCleared;
                        fTruck.Status.GPSIssueAlarmExcused = t.Status.GPSIssueAlarmExcused;
                        fTruck.Status.GPSIssueAlarmComments = t.Status.GPSIssueAlarmComments;
                        fTruck.Status.StationaryAlarm = t.Status.StationaryAlarm; //handles no movement, speed = 0
                        fTruck.Status.StationaryAlarmStart = t.Status.StationaryAlarmStart; //handles no movement, speed = 0
                        fTruck.Status.StationaryAlarmCleared = t.Status.StationaryAlarmCleared;
                        fTruck.Status.StationaryAlarmExcused = t.Status.StationaryAlarmExcused;
                        fTruck.Status.StationaryAlarmComments = t.Status.StationaryAlarmComments;
                        //update driver
                        fTruck.Driver.DriverID = t.Driver.DriverID;
                        fTruck.Driver.LastName = t.Driver.LastName;
                        fTruck.Driver.FirstName = t.Driver.FirstName;
                        fTruck.Driver.TowTruckCompany = t.Driver.TowTruckCompany;
                        fTruck.Driver.FSPID = t.Driver.FSPID;
                        fTruck.Driver.AssignedBeat = t.Driver.AssignedBeat;
                        fTruck.Driver.BeatScheduleID = t.Driver.BeatScheduleID;
                        fTruck.Driver.BreakStarted = t.Driver.BreakStarted;
                        fTruck.Driver.LunchStarted = t.Driver.LunchStarted;
                        //Extended data
                        fTruck.Extended.ContractorName = t.Extended.ContractorName;
                        fTruck.Extended.ContractorID = t.Extended.ContractorID;
                        fTruck.Extended.TruckNumber = t.Extended.TruckNumber;
                        fTruck.Extended.FleetNumber = t.Extended.FleetNumber;
                        fTruck.Extended.ProgramStartDate = t.Extended.ProgramStartDate;
                        fTruck.Extended.VehicleType = t.Extended.VehicleType;
                        fTruck.Extended.VehicleYear = t.Extended.VehicleYear;
                        fTruck.Extended.VehicleMake = t.Extended.VehicleMake;
                        fTruck.Extended.VehicleModel = t.Extended.VehicleModel;
                        fTruck.Extended.LicensePlate = t.Extended.LicensePlate;
                        fTruck.Extended.RegistrationExpireDate = t.Extended.RegistrationExpireDate;
                        fTruck.Extended.InsuranceExpireDate = t.Extended.InsuranceExpireDate;
                        fTruck.Extended.LastCHPInspection = t.Extended.LastCHPInspection;
                        fTruck.Extended.ProgramEndDate = t.Extended.ProgramEndDate;
                        fTruck.Extended.FAW = t.Extended.FAW;
                        fTruck.Extended.RAW = t.Extended.RAW;
                        fTruck.Extended.RAWR = t.Extended.RAWR;
                        fTruck.Extended.GVW = t.Extended.GVW;
                        fTruck.Extended.GVWR = t.Extended.GVWR;
                        fTruck.Extended.Wheelbase = t.Extended.Wheelbase;
                        fTruck.Extended.Overhang = t.Extended.Overhang;
                        fTruck.Extended.MAXTW = t.Extended.MAXTW;
                        fTruck.Extended.MAXTWCALCDATE = t.Extended.MAXTWCALCDATE;
                        fTruck.Extended.FuelType = t.Extended.FuelType;
                        fTruck.Extended.FleetVehicleID = t.Extended.FleetVehicleID;
                    }
                }
            }
            catch (Exception ex)
            {
                logger = new Logging.EventLogger();
                logger.LogEvent("Error receiving truck dump" + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void TruckDump(string truckList)
        {
            try
            {
                JsonSerializer des = new JsonSerializer();

                //List<TowTruck.TowTruck> trucks = des.Deserialize<List<TowTruck.TowTruck>>(truckList);
                List<TowTruck.TowTruck> trucks = JsonConvert.DeserializeObject<List<TowTruck.TowTruck>>(truckList);
                foreach (TowTruck.TowTruck t in trucks)
                {
                    TowTruck.TowTruck thisFoundTruck = DataClasses.GlobalData.currentTrucks.Find(delegate (TowTruck.TowTruck truck) { return truck.Identifier == t.Identifier; });
                    if (thisFoundTruck == null)
                    {
                        DataClasses.GlobalData.currentTrucks.Add(t);
                    }
                    else
                    {
                        if (t.Status != null)
                        {
                            thisFoundTruck.Status = t.Status;
                        }
                        if (t.Extended != null)
                        {
                            thisFoundTruck.Extended = t.Extended;
                        }
                        if (t.Driver != null)
                        {
                            thisFoundTruck.Driver = t.Driver;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public void SetSpeedingValue(int NewSpeed)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            mySQL.SetVarValue("Speeding", NewSpeed.ToString());
            DataClasses.GlobalData.SpeedingValue = NewSpeed;
        }

        #region " find, set and remove incidents "

        public List<IncidentScreenData> GetAllIncidents()
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            List<IncidentScreenData> AllIncidents = new List<IncidentScreenData>();
            //since we can have multiple assists per incident, we need to loop through incidents and assists associated with that incident to build the result set
            foreach (MiscData.Incident thisIncident in DataClasses.GlobalData.Incidents)
            {
                List<MiscData.Assist> theseAssists = DataClasses.GlobalData.GetAssistsByIncident(thisIncident.IncidentID);
                if (theseAssists.Count() > 0) //we have assists associated with incident
                {
                    foreach (MiscData.Assist thisAssist in theseAssists)
                    {
                        AllIncidents.Add(new IncidentScreenData
                        {
                            IncidentID = thisIncident.IncidentID,
                            Direction = thisIncident.Direction,
                            Location = thisIncident.Location,
                            Freeway = DataClasses.GlobalData.FindFreewayNameByID(thisIncident.FreewayID),
                            TimeStamp = thisIncident.TimeStamp.ToString(),
                            CreatedBy = mySQL.FindUserNameByID(thisIncident.CreatedBy),
                            Description = thisIncident.Description,
                            IncidentNumber = thisIncident.IncidentNumber,
                            CrossStreet1 = thisIncident.CrossStreet1,
                            CrossStreet2 = thisIncident.CrossStreet2,
                            BeatNumber = thisIncident.BeatNumber,
                            TruckNumber = DataClasses.GlobalData.FindTruckNumberByID(thisAssist.FleetVehicleID),
                            Driver = mySQL.FindDriverNameByID(thisAssist.DriverID),
                            State = DataClasses.GlobalData.FindTowTruckStatusByID(thisAssist.FleetVehicleID),
                            ContractorName = DataClasses.GlobalData.FindContractorByTruckNumber(DataClasses.GlobalData.FindTruckNumberByID(thisAssist.FleetVehicleID))
                        });
                    }
                }
                else
                {
                    AllIncidents.Add(new IncidentScreenData
                    {
                        IncidentID = thisIncident.IncidentID,
                        Direction = thisIncident.Direction,
                        Location = thisIncident.Location,
                        Freeway = DataClasses.GlobalData.FindFreewayNameByID(thisIncident.FreewayID),
                        TimeStamp = thisIncident.TimeStamp.ToString(),
                        CreatedBy = mySQL.FindUserNameByID(thisIncident.CreatedBy),
                        Description = thisIncident.Description,
                        IncidentNumber = thisIncident.IncidentNumber,
                        CrossStreet1 = thisIncident.CrossStreet1,
                        CrossStreet2 = thisIncident.CrossStreet2,
                        BeatNumber = thisIncident.BeatNumber,
                        TruckNumber = "No Assist Assigned",
                        Driver = "No Assist Assigned",
                        State = "No Assist Assigned"
                    });
                }

            }

            mySQL = null;
            return AllIncidents;
        }

        //public List<IncidentIn> GetIncidents()
        //{
        //    List<IncidentIn> AllIncidents = new List<IncidentIn>();
        //    foreach (MiscData.Incident thisIncident in DataClasses.GlobalData.Incidents)
        //    {
        //        AllIncidents.Add(new IncidentIn { 
        //            IncidentID = thisIncident.IncidentID,
        //            Location = thisIncident.Location,
        //            FreewayID = thisIncident.FreewayID,
        //            LocationID = thisIncident.LocationID,
        //            BeatNumber = thisIncident.BeatNumber,
        //            TimeStamp = thisIncident.TimeStamp,
        //            CreatedBy = thisIncident.CreatedBy,
        //            Description = thisIncident.Description,
        //            IncidentNumber = thisIncident.IncidentNumber
        //        });
        //    }
        //    return AllIncidents;
        //}

        public IncidentIn FindIncidentByID(Guid IncidentID)
        {
            MiscData.Incident thisIncident = DataClasses.GlobalData.FindIncidentByID(IncidentID);
            if (thisIncident == null)
            {
                return null;
            }
            else
            {
                IncidentIn thisIncidentIn = new IncidentIn();
                thisIncidentIn.IncidentID = thisIncident.IncidentID;
                thisIncidentIn.Location = thisIncident.Location;
                thisIncidentIn.FreewayID = thisIncident.FreewayID;
                thisIncidentIn.LocationID = thisIncident.LocationID;
                thisIncidentIn.BeatNumber = thisIncident.BeatNumber;
                thisIncidentIn.TimeStamp = thisIncident.TimeStamp;
                thisIncidentIn.Description = thisIncident.Description;
                thisIncidentIn.IncidentNumber = thisIncident.IncidentNumber;
                return thisIncidentIn;
            }
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

        public void AddIncident(IncidentIn thisIncidentIn)
        {

            //string DispatchNumber = GenerateNumber("i");

            MiscData.Incident thisIncident = new MiscData.Incident();
            thisIncident.Direction = thisIncidentIn.Direction;
            thisIncident.IncidentID = thisIncidentIn.IncidentID;
            thisIncident.Location = thisIncidentIn.Location;
            thisIncident.FreewayID = thisIncidentIn.FreewayID;
            thisIncident.LocationID = thisIncidentIn.LocationID;
            thisIncident.BeatNumber = thisIncidentIn.BeatNumber;
            thisIncident.TimeStamp = thisIncidentIn.TimeStamp;
            thisIncident.CreatedBy = thisIncidentIn.CreatedBy;
            thisIncident.Description = thisIncidentIn.Description;
            //thisIncident.IncidentNumber = thisIncidentIn.IncidentNumber;
            thisIncident.CrossStreet1 = thisIncidentIn.CrossStreet1;
            thisIncident.CrossStreet2 = thisIncidentIn.CrossStreet2;
            thisIncident.IncidentNumber = GenerateNumber("i");
            DataClasses.GlobalData.AddIncident(thisIncident);
        }

        public void ClearIncident(Guid IncidentID)
        {
            DataClasses.GlobalData.ClearIndicent(IncidentID);
        }

        #endregion

        #region " find, set and remove assists "

        public void AddAssist(AssistReq thisReq)
        {
            //string DispatchNumber = GenerateNumber("a");

            MiscData.Assist thisRequest = new MiscData.Assist();
            thisRequest.AssistID = thisReq.AssistID;
            thisRequest.AssistNumber = GenerateNumber("a");
            thisRequest.IncidentID = thisReq.IncidentID;
            thisRequest.FleetVehicleID = thisReq.FleetVehicleID;
            thisRequest.DispatchTime = thisReq.DispatchTime;
            //thisRequest.ServiceTypeID = thisReq.ServiceTypeID;
            thisRequest.DropZone = thisReq.DropZone;
            thisRequest.Make = thisReq.Make;
            thisRequest.VehicleTypeID = thisReq.VehicleTypeID;
            thisRequest.VehiclePositionID = thisReq.VehiclePositionID;
            thisRequest.Color = thisReq.Color;
            thisRequest.LicensePlate = thisReq.LicensePlate;
            thisRequest.State = thisReq.State;
            thisRequest.StartOD = thisReq.StartOD;
            thisRequest.EndOD = thisReq.EndOD;
            thisRequest.TowLocationID = thisReq.TowLocationID;
            thisRequest.Tip = thisReq.Tip;
            thisRequest.TipDetail = thisReq.TipDetail;
            thisRequest.CustomerLastName = thisReq.CustomerLastName;
            thisRequest.Comments = thisReq.Comments;
            thisRequest.IsMDC = thisReq.IsMDC;
            thisRequest.x1097 = thisReq.x1097;
            thisRequest.x1098 = thisReq.x1098;
            thisRequest.ContractorID = thisReq.ContractorID;
            thisRequest.LogNumber = thisReq.LogNumber;
            thisRequest.DriverID = FindDriverID(thisReq.FleetVehicleID);
            thisRequest.Lat = thisReq.Lat;
            thisRequest.Lon = thisRequest.Lon;
            thisRequest.AssistComplete = false;
            //Make sure incident has correct beat number
            DataClasses.GlobalData.UpdateIncidentBeat(thisReq.IncidentID, thisReq.FleetVehicleID);
            DataClasses.GlobalData.AddAssist(thisRequest);
        }

        private Guid FindDriverID(Guid TruckID)
        {
            Guid DriverID = new Guid("00000000-0000-0000-0000-000000000000");
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruckByTruckID(TruckID);
            if (thisTruck != null)
            {
                DriverID = thisTruck.Driver.DriverID;
            }
            return DriverID;
        }

        public List<AssistScreenData> GetAllAssists()
        {
            List<AssistScreenData> AllAssists = new List<AssistScreenData>();
            SQL.SQLCode mySQL = new SQL.SQLCode();
            string[] ServiceText;
            foreach (MiscData.Assist thisAssist in DataClasses.GlobalData.Assists)
            {
                if (thisAssist.SelectedServices != null && thisAssist.SelectedServices.Count() > 0)
                {
                    ServiceText = new string[thisAssist.SelectedServices.Count()];
                    for (int i = 0; i < thisAssist.SelectedServices.Count(); i++)
                    {
                        ServiceText[i] = DataClasses.GlobalData.FindServiceTypeNameByID(new Guid(thisAssist.SelectedServices[i].ToString()));
                    }
                }
                else
                {
                    ServiceText = new string[1];
                    ServiceText[0] = "No Services Rendered";
                }
                AllAssists.Add(new AssistScreenData
                {
                    AssistID = thisAssist.AssistID,
                    DriverName = mySQL.FindDriverNameByID(thisAssist.DriverID),
                    DispatchNumber = DataClasses.GlobalData.FindIncidentNumberByID(thisAssist.IncidentID),
                    AssistNumber = thisAssist.AssistNumber,
                    IncidentNumber = DataClasses.GlobalData.FindIncidentNumberByID(thisAssist.IncidentID),
                    BeatNumber = DataClasses.GlobalData.FindBeatNumberByID(thisAssist.IncidentID),
                    VehicleNumber = DataClasses.GlobalData.FindTruckNumberByID(thisAssist.FleetVehicleID),
                    ContractorName = DataClasses.GlobalData.FindContractorNameByID(thisAssist.ContractorID),
                    x1097 = thisAssist.x1097,
                    OnSiteTime = thisAssist.OnSiteTime,
                    x0198 = thisAssist.x1098,
                    Comments = thisAssist.Comments,
                    Latitude = thisAssist.Lat,
                    Longitude = thisAssist.Lon,
                    CustomerWaitTime = thisAssist.CustomerWaitTime,
                    VehiclePosition = DataClasses.GlobalData.FindVehiclePositionNameByID(thisAssist.VehiclePositionID),
                    TrafficSpeed = DataClasses.GlobalData.FindTrafficSpeedNameByID(thisAssist.TrafficSpeedID),
                    DropZone = thisAssist.DropZone,
                    Make = thisAssist.Make,
                    VehicleType = DataClasses.GlobalData.FindVehicleTypeNameByID(thisAssist.VehicleTypeID),
                    Color = thisAssist.Color,
                    LicensePlate = thisAssist.LicensePlate,
                    State = DataClasses.GlobalData.FindTowTruckStatusByID(thisAssist.FleetVehicleID),
                    StartOD = thisAssist.StartOD,
                    EndOD = thisAssist.EndOD,
                    TowLocation = DataClasses.GlobalData.FindTowLocationNameByID(thisAssist.TowLocationID),
                    Tip = thisAssist.Tip,
                    TipDetail = thisAssist.TipDetail,
                    CustomerLastName = thisAssist.CustomerLastName,
                    SelectedServices = ServiceText,
                    AssistComplete = thisAssist.AssistComplete,
                    AssistAcked = thisAssist.Acked
                });
            }

            return AllAssists;
        }



        /*
        public void AddTruckAssistRequest(string IPAddress, AssistReq thisReq, Guid IncidentID)
        {
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(IPAddress);
            //TODO: Needs to be fixed
            if (thisTruck != null)
            {
                MiscData.Assist thisRequest = new MiscData.Assist();
                thisRequest.AssistID = thisReq.AssistID;
                thisRequest.IncidentID = thisReq.IncidentID;
                thisRequest.FleetVehicleID = thisReq.FleetVehicleID;
                thisRequest.DispatchTime = thisReq.DispatchTime;
                thisRequest.ServiceTypeID = thisReq.ServiceTypeID;
                thisRequest.DropZone = thisReq.DropZone;
                thisRequest.Make = thisReq.Make;
                thisRequest.VehicleTypeID = thisReq.VehicleTypeID;
                thisRequest.VehiclePositionID = thisReq.VehiclePositionID;
                thisRequest.Color = thisReq.Color;
                thisRequest.LicensePlate = thisReq.LicensePlate;
                thisRequest.State = thisReq.State;
                thisRequest.StartOD = thisReq.StartOD;
                thisRequest.EndOD = thisReq.EndOD;
                thisRequest.TowLocationID = thisReq.TowLocationID;
                thisRequest.Tip = thisReq.Tip;
                thisRequest.TipDetail = thisReq.TipDetail;
                thisRequest.CustomerLastName = thisReq.CustomerLastName;
                thisRequest.Comments = thisReq.Comments;
                thisRequest.IsMDC = thisReq.IsMDC;
                thisRequest.x1097 = thisReq.x1097;
                thisRequest.x1098 = thisReq.x1098;
                thisRequest.ContractorID = thisReq.ContractorID;
                thisRequest.LogNumber = thisReq.LogNumber;
                thisTruck.AddAssistRequest(thisRequest);

                DataClasses.GlobalData.AssignTruckToAssist(IncidentID, thisTruck);
            }
        }

        public void ClearTruckAssistRequest(string IPAddress, Guid AssistRequestID)
        {
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(IPAddress);
            if (thisTruck != null)
            {
                thisTruck.ClearAssistRequest(AssistRequestID);
            }
        }
        */
        #endregion

        #region " Geo-Objects "

        #region Segments CRUD

        public string DeleteSegment(Guid segmentid)
        {

            SQL.SQLCode sql = new SQL.SQLCode();
            string result = sql.DeleteSegment(segmentid);
            BeatData.Beats.LoadBeatSegments();

            return result;
        }

        public string CreateSegment(BeatSegment_New segment)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(segment.BeatSegmentExtent);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            segment.BeatSegmentExtent = extstring;
            string result = sql.CreateSegment(segment);
            BeatData.Beats.LoadBeatSegments();

            return result;
        }

        public string UpdateSegment(BeatSegment_New segment)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(segment.BeatSegmentExtent);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            segment.BeatSegmentExtent = extstring;
            string result = sql.UpdateSegment(segment);
            BeatData.Beats.LoadBeatSegments();

            return result;
        }

        public List<BeatSegment_New> RetreiveAllSegments()
        {
            List<BeatSegment_New> Segments = new List<BeatSegment_New>();
            SQL.SQLCode sql = new SQL.SQLCode();
            Segments = sql.RetrieveAllSegments();
            foreach (BeatSegment_New bn in Segments)
            {
                string JSON = "[";
                string[] extent = bn.BeatSegmentExtent.Split(',');
                for (int i = 0; i < extent.Length; i++)
                {
                    string[] LL = extent[i].Trim().Split(' ');
                    if (i == extent.Length - 1)
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                    }
                    else
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                    }
                }
                JSON += "]";
                bn.BeatSegmentExtent = JSON;

                if(bn.Color == null || bn.Color == "")
                {
                    bn.Color = "#000000";
                }
            }

            return Segments;
        }

        public BeatSegment_New RetrieveSegment(Guid SegmentID)
        {
            BeatSegment_New Beat = new BeatSegment_New();
            SQL.SQLCode sql = new SQL.SQLCode();
            Beat = sql.RetrieveSegment(SegmentID);
            string JSON = "[";
            string[] extent = Beat.BeatSegmentExtent.Split(',');
            for (int i = 0; i < extent.Length; i++)
            {
                string[] LL = extent[i].Trim().Split(' ');
                if (i == extent.Length - 1)
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                }
                else
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                }
            }
            JSON += "]";
            Beat.BeatSegmentExtent = JSON;

            if (Beat.Color == null || Beat.Color == "")
            {
                Beat.Color = "#000000";
            }

            return Beat;
        }

        #endregion

        #region Beats CRUD

        public string DeleteBeat(Guid BeatID)
        {

            SQL.SQLCode sql = new SQL.SQLCode();
            string result = sql.DeleteBeat(BeatID);
            BeatData.Beats.LoadBeats();

            return result;
        }

        public string CreateBeat(Beats_New beat)
        {
            beat.StartDate = DateTime.Now;
            beat.EndDate = DateTime.Now.AddYears(25);
            beat.FreewayID = 0;

            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(beat.BeatExtent);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            beat.BeatExtent = extstring;
            string result = sql.CreateBeat(beat);
            BeatData.Beats.LoadBeats();

            return result;
        }

        public string UpdateBeat(Beats_New beat)
        {
            beat.StartDate = DateTime.Now;
            beat.EndDate = DateTime.Now.AddYears(25);
            beat.FreewayID = 0;

            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(beat.BeatExtent);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            beat.BeatExtent = extstring;
            string result = sql.CreateBeat(beat);
            BeatData.Beats.LoadBeats();

            return result;
        }

        public List<Beats_New> RetreiveAllBeats()
        {
            List<Beats_New> Beats = new List<Beats_New>();
            SQL.SQLCode sql = new SQL.SQLCode();
            Beats = sql.RetrieveAllBeats();
            foreach (Beats_New bn in Beats)
            {
                string JSON = "[";
                string[] extent = bn.BeatExtent.Split(',');
                for (int i = 0; i < extent.Length; i++)
                {
                    string[] LL = extent[i].Trim().Split(' ');
                    if (i == extent.Length - 1)
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                    }
                    else
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                    }
                }
                JSON += "]";
                bn.BeatExtent = JSON;
                bn.BeatSegments = new List<BeatSegment_New>();
                if(bn.BeatColor == null || bn.BeatColor == "")
                {
                    bn.BeatColor = "#000000";
                }
                bn.BeatSegments = sql.RetrieveBeatSegments(bn.BeatID);

                //Get this into JSON object for Tolga
                foreach (BeatSegment_New bsn in bn.BeatSegments)
                {
                    string JSON2 = "[";
                    string[] extent2 = bsn.BeatSegmentExtent.Split(',');
                    for (int x = 0; x < extent2.Length; x++)
                    {
                        if (x == extent.Length - 1)
                        {
                            JSON2 += "{ lat: " + extent2[0] + ", lng: " + extent2[1] + " }";
                        }
                        else
                        {
                            JSON2 += "{ lat: " + extent2[0] + ", lng: " + extent2[1] + " },";
                        }
                    }

                    JSON2 += "]";
                    bsn.BeatSegmentExtent = JSON2;
                }
            }

            return Beats;
        }

        public Beats_New RetreiveBeat(Guid BeatID)
        {
            Beats_New Beat = new Beats_New();
            SQL.SQLCode sql = new SQL.SQLCode();
            Beat = sql.RetrieveBeat(BeatID);
            string JSON = "[";
            string[] extent = Beat.BeatExtent.Split(',');
            for (int i = 0; i < extent.Length; i++)
            {
                string[] LL = extent[i].Trim().Split(' ');
                if (i == extent.Length - 1)
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                }
                else
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                }
            }
            JSON += "]";
            Beat.BeatExtent = JSON;
            if (Beat.BeatColor == null || Beat.BeatColor == "")
            {
                Beat.BeatColor = "#000000";
            }
            Beat.BeatSegments = new List<BeatSegment_New>();
            Beat.BeatSegments = sql.RetrieveBeatSegments(Beat.BeatID);

            //Get this into JSON object for Tolga
            foreach (BeatSegment_New bsn in Beat.BeatSegments)
            {
                string JSON2 = "[";
                string[] extent2 = bsn.BeatSegmentExtent.Split(',');
                for (int x = 0; x < extent2.Length; x++)
                {
                    if (x == extent.Length - 1)
                    {
                        JSON2 += "{ lat: " + extent2[0] + ", lng: " + extent2[1] + " }";
                    }
                    else
                    {
                        JSON2 += "{ lat: " + extent2[0] + ", lng: " + extent2[1] + " },";
                    }
                }

                JSON2 += "]";
                bsn.BeatSegmentExtent = JSON2;
            }

            return Beat;
        }

        #endregion

        #region Tow Truck Yards

        public string DeleteYard(Guid YardID)
        {

            SQL.SQLCode sql = new SQL.SQLCode();
            string result = sql.DeleteYard(YardID);
            BeatData.YardClass.LoadYards();

            return result;
        }

        public string CreateYard(Yard_New yard)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(yard.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            yard.Position = extstring;
            string result = sql.UpdateYard(yard);
            BeatData.YardClass.LoadYards();

            return result;
        }

        public string UpdateYard(Yard_New yard)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(yard.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            yard.Position = extstring;
            string result = sql.UpdateYard(yard);
            BeatData.YardClass.LoadYards();

            return result;
        }

        public List<Yard_New> RetreiveAllYards()
        {
            List<Yard_New> Yards = new List<Yard_New>();
            SQL.SQLCode sql = new SQL.SQLCode();
            Yards = sql.RetrieveAllYards();
            foreach (Yard_New yard in Yards)
            {
                string JSON = "[";
                string[] extent = yard.Position.Split(',');
                for (int i = 0; i < extent.Length; i++)
                {
                    string[] LL = extent[i].Trim().Split(' ');
                    if (i == extent.Length - 1)
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                    }
                    else
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                    }
                }
                JSON += "]";
                yard.Position = JSON;
            }

            return Yards;
        }

        public Yard_New RetreiveYard(Guid TowTruckYardID)
        {
            Yard_New Yard = new Yard_New();
            SQL.SQLCode sql = new SQL.SQLCode();
            Yard = sql.RetrieveYard(TowTruckYardID);
            string JSON = "[";
            string[] extent = Yard.Position.Split(',');
            for (int i = 0; i < extent.Length; i++)
            {
                string[] LL = extent[i].Trim().Split(' ');
                if (i == extent.Length - 1)
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                }
                else
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                }
            }
            JSON += "]";
            Yard.Position = JSON;

            return Yard;
        }

        #endregion

        #region DropZones CRUD

        public string DeleteDropZone(Guid DropZoneID)
        {

            SQL.SQLCode sql = new SQL.SQLCode();
            string result = sql.DeleteDropZone(DropZoneID);
            sql.LoadDropZones();

            return result;
        }

        public string CreateDropZone(DropZone_New DropZone)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(DropZone.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            DropZone.Position = extstring;
            string result = sql.UpdateDropZone(DropZone);
            sql.LoadDropZones();

            return result;
        }

        public string UpdateDropZone(DropZone_New DropZone)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(DropZone.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            DropZone.Position = extstring;
            string result = sql.UpdateDropZone(DropZone);
            sql.LoadDropZones();

            return result;
        }

        public List<DropZone_New> RetreiveAllDZs()
        {
            List<DropZone_New> DZones = new List<DropZone_New>();
            SQL.SQLCode sql = new SQL.SQLCode();
            DZones = sql.RetreiveAllDZs();
            foreach (DropZone_New zone in DZones)
            {
                string JSON = "[";
                string[] extent = zone.Position.Split(',');
                for (int i = 0; i < extent.Length; i++)
                {
                    string[] LL = extent[i].Trim().Split(' ');
                    if (i == extent.Length - 1)
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                    }
                    else
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                    }
                }
                JSON += "]";
                zone.Position = JSON;
            }

            return DZones;
        }

        public DropZone_New RetreiveDZ(Guid DropZoneID)
        {
            DropZone_New DZ = new DropZone_New();
            SQL.SQLCode sql = new SQL.SQLCode();
            DZ = sql.RetreiveDropZone(DropZoneID);
            string JSON = "[";
            string[] extent = DZ.Position.Split(',');
            for (int i = 0; i < extent.Length; i++)
            {
                string[] LL = extent[i].Trim().Split(' ');
                if (i == extent.Length - 1)
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                }
                else
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                }
            }
            JSON += "]";
            DZ.Position = JSON;

            return DZ;
        }

        #endregion

        #region CallBoxes CRUD

        public string DeleteCallBox(Guid CallBoxID)
        {

            SQL.SQLCode sql = new SQL.SQLCode();
            string result = sql.DeleteCallBox(CallBoxID);
            sql.LoadDropZones();

            return result;
        }

        public string CreateCallBox(CallBoxes_New CallBox)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(CallBox.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            CallBox.Position = extstring;
            string result = sql.UpdateCallBox(CallBox);

            return result;
        }

        public string UpdateCallBox(CallBoxes_New CallBox)
        {
            string extstring = "";
            SQL.SQLCode sql = new SQL.SQLCode();
            List<latLng> ext = JsonConvert.DeserializeObject<List<latLng>>(CallBox.Position);
            for (int i = 0; i < ext.Count; i++)
            {
                if (i < ext.Count - 1)
                {
                    extstring += ext[i].lat + " " + ext[i].lng + ", ";
                }
                else
                {
                    extstring += ext[i].lat + " " + ext[i].lng;
                }
            }
            CallBox.Position = extstring;
            string result = sql.UpdateCallBox(CallBox);

            return result;
        }

        public List<CallBoxes_New> RetreiveCallBoxes()
        {
            List<CallBoxes_New> CallBoxes = new List<CallBoxes_New>();
            SQL.SQLCode sql = new SQL.SQLCode();
            CallBoxes = sql.RetreiveCallBoxes();
            foreach (CallBoxes_New callBox in CallBoxes)
            {
                string JSON = "[";
                string[] extent = callBox.Position.Split(',');
                for (int i = 0; i < extent.Length; i++)
                {
                    string[] LL = extent[i].Trim().Split(' ');
                    if (i == extent.Length - 1)
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                    }
                    else
                    {
                        JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                    }
                }
                JSON += "]";
                callBox.Position = JSON;
            }

            return CallBoxes;
        }

        public CallBoxes_New RetreiveCallBox(Guid CallBoxID)
        {
            CallBoxes_New CallBox = new CallBoxes_New();
            SQL.SQLCode sql = new SQL.SQLCode();
            CallBox = sql.RetreiveCallBox(CallBoxID);
            string JSON = "[";
            string[] extent = CallBox.Position.Split(',');
            for (int i = 0; i < extent.Length; i++)
            {
                string[] LL = extent[i].Trim().Split(' ');
                if (i == extent.Length - 1)
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " }";
                }
                else
                {
                    JSON += "{ lat: " + LL[0] + ", lng: " + LL[1] + " },";
                }
            }
            JSON += "]";
            CallBox.Position = JSON;

            return CallBox;
        }

        #endregion

        #endregion

        public int GetUsedBreakTime(string DriverID, string Type)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            return mySQL.GetUsedBreakTime(DriverID, Type);
        }

        public string[] GetPreloadedData(string Type)
        {
            string[] DataOut;
            int TypeCount = 0;
            switch (Type)
            {
                case "Code1098s":
                    TypeCount = DataClasses.GlobalData.Code1098s.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.Code1098s.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.Code1098s[i].CodeName.ToString();
                    }
                    break;
                case "Freeways":
                    TypeCount = DataClasses.GlobalData.Freeways.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.Freeways.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.Freeways[i].FreewayName.ToString();
                    }
                    break;
                case "IncidentTypes":
                    TypeCount = DataClasses.GlobalData.IncidentTypes.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.IncidentTypes.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.IncidentTypes[i].IncidentTypeCode.ToString();
                    }
                    break;
                case "LocationCodes":
                    TypeCount = DataClasses.GlobalData.LocationCodes.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.LocationCodes.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.LocationCodes[i].LocationCode.ToString();
                    }
                    break;
                case "ServiceTypes":
                    TypeCount = DataClasses.GlobalData.ServiceTypes.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.ServiceTypes.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.ServiceTypes[i].ServiceTypeCode.ToString();
                    }
                    break;
                case "TowLocations":
                    TypeCount = DataClasses.GlobalData.TowLocations.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.TowLocations.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.TowLocations[i].TowLocationCode.ToString();
                    }
                    break;
                case "TrafficSpeeds":
                    TypeCount = DataClasses.GlobalData.TrafficSpeeds.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.TrafficSpeeds.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.TrafficSpeeds[i].TrafficSpeedCode.ToString();
                    }
                    break;
                case "VehiclePositions":
                    TypeCount = DataClasses.GlobalData.VehiclePositions.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.VehiclePositions.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.VehiclePositions[i].VehiclePositionCode.ToString();
                    }
                    break;
                case "VehicleTypes":
                    TypeCount = DataClasses.GlobalData.VehicleTypes.Count;
                    DataOut = new string[TypeCount];
                    for (int i = 0; i < DataClasses.GlobalData.VehicleTypes.Count; i++)
                    {
                        DataOut[i] = DataClasses.GlobalData.VehicleTypes[i].VehicleTypeCode.ToString();
                    }
                    break;
                default:
                    DataOut = new string[1];
                    DataOut[0] = "NO DATA";
                    break;
            }
            return DataOut;
        }

        public List<AssistTruck> GetAssistTrucks()
        {
            List<AssistTruck> myTrucks = new List<AssistTruck>();
            foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks)
            {
                if (thisTruck.Driver.LastName != "Not Logged On")
                {
                    AssistTruck thisAssistTruck = new AssistTruck();
                    thisAssistTruck.TruckID = thisTruck.Extended.FleetVehicleID;
                    thisAssistTruck.TruckNumber = thisTruck.Extended.TruckNumber;
                    thisAssistTruck.ContractorID = thisTruck.Extended.ContractorID;
                    thisAssistTruck.ContractorName = thisTruck.Extended.ContractorName;
                    myTrucks.Add(thisAssistTruck);
                }
            }
            return myTrucks;
        }

        public List<IncidentDisplay> getIncidentData()
        {
            List<IncidentDisplay> idl = new List<IncidentDisplay>();
            SQL.SQLCode mySQL = new SQL.SQLCode();
            foreach (MiscData.Assist ta in DataClasses.GlobalData.Assists)
            {
                MiscData.Incident inc = DataClasses.GlobalData.FindIncidentByID(ta.IncidentID);
                TowTruck.TowTruck tt = DataClasses.GlobalData.FindTowTruckByTruckID(ta.FleetVehicleID);

                string State = "Not Connected";
                if (tt != null)
                {
                    State = tt.Status.VehicleStatus;
                }

                if (inc != null)
                {
                    IncidentDisplay id = new IncidentDisplay();
                    id.IncidentID = inc.IncidentID;
                    id.IncidentNumber = inc.IncidentNumber;
                    id.AssistNumber = ta.AssistNumber;
                    id.BeatNumber = inc.BeatNumber;
                    //id.TruckNumber = tt.TruckNumber;
                    //id.DriverName = tt.Driver.LastName + ", " + tt.Driver.FirstName;
                    id.TruckNumber = mySQL.GetTruckNumberByID(ta.FleetVehicleID);
                    id.DriverName = mySQL.FindDriverNameByID(ta.DriverID);
                    id.DispatchComments = inc.Description;
                    id.Timestamp = inc.TimeStamp;
                    id.DispatchNumber = inc.IncidentNumber;
                    //id.ContractorName = tt.Extended.ContractorName;
                    id.ContractorName = DataClasses.GlobalData.FindContractorNameByID(ta.ContractorID);
                    id.IsIncidentComplete = ta.AssistComplete;
                    id.State = State;
                    id.IsAcked = ta.Acked;
                    idl.Add(id);
                }
            }

            return idl;
        }

        public List<TowTruckData> CurrentTrucks()
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
                    Location = thisTruck.currentBeatSegment, //needs to show segment, description of segment
                    StatusStarted = thisTruck.Status.StatusStarted
                });
            }
            /*
            var noDrv = from nd in DataClasses.GlobalData.currentTrucks
                        where nd.Driver.LastName == "Not Logged On"
                        select nd;
            foreach(TowTruck.TowTruck thisTruck in noDrv)
            {
                bool HasAlarms = false;
                if (thisTruck.Status.OutOfBoundsAlarm == true || thisTruck.Status.SpeedingAlarm == true)
                { HasAlarms = true; }
                myTrucks.Add(new TowTruckData { 
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
                    BeatNumber = new Guid("00000000-0000-0000-0000-000000000000")
                });
            }
            var Drv = from d in DataClasses.GlobalData.currentTrucks
                        where d.Driver.LastName != "Not Logged On"
                        select d;
            foreach (TowTruck.TowTruck thisTruck in Drv)
            {
                bool HasAlarms = false;
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
                    BeatID = new Guid("00000000-0000-0000-0000-000000000000")
                });
            }
             * */
            return myTrucks;
        }

        public List<AlarmStatus> GetAllAlarms()
        {
            try
            {
                List<AlarmStatus> AllAlarms = new List<AlarmStatus>();

                var tList = from t in DataClasses.GlobalData.currentTrucks
                            where t.Status.VehicleStatus != "Waiting for Driver Login"
                            select t;

                foreach (TowTruck.TowTruck thisTruck in tList)
                {
                    AllAlarms.Add(new AlarmStatus
                    {
                        BeatNumber = thisTruck.assignedBeat.BeatNumber.ToString(),
                        VehicleNumber = thisTruck.Extended.TruckNumber,
                        DriverName = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName,
                        ContractCompanyName = thisTruck.Extended.ContractorName,
                        SpeedingAlarm = thisTruck.Status.SpeedingAlarm,
                        SpeedingValue = thisTruck.Status.SpeedingValue,
                        SpeedingTime = thisTruck.Status.SpeedingTime,
                        OutOfBoundsAlarm = thisTruck.Status.OutOfBoundsAlarm,
                        OutOfBoundsMessage = thisTruck.Status.OutOfBoundsMessage,
                        OutOfBoundsTime = thisTruck.Status.OutOfBoundsTime,
                        OutOfBoundsStartTime = thisTruck.Status.OutOfBoundsStartTime,
                        OutOfBoundsCleared = thisTruck.Status.OutOfBoundsTimeCleared,
                        OutOfBoundsExcused = thisTruck.Status.OutOfBoundsTimeExcused,
                        VehicleStatus = thisTruck.Status.VehicleStatus,
                        StatusStarted = thisTruck.Status.StatusStarted,
                        LogOnAlarm = thisTruck.Status.LogOnAlarm,
                        LogOnAlarmTime = thisTruck.Status.LogOnAlarmTime,
                        LogOnAlarmCleared = thisTruck.Status.LogOnAlarmCleared,
                        LogOnAlarmExcused = thisTruck.Status.LogOnAlarmCleared,
                        LogOnAlarmComments = thisTruck.Status.LogOnAlarmComments,
                        RollInAlarm = thisTruck.Status.RollInAlarm,
                        RollInAlarmTime = thisTruck.Status.RollInAlarmTime,
                        RollInAlarmCleared = thisTruck.Status.RollInAlarmCleared,
                        RollInAlarmExcused = thisTruck.Status.RollInAlarmExcused,
                        RollInAlarmComments = thisTruck.Status.RollInAlarmComments,
                        RollOutAlarm = thisTruck.Status.RollOutAlarm,
                        RollOutAlarmTime = thisTruck.Status.RollOutAlarmTime,
                        RollOutAlarmCleared = thisTruck.Status.RollOutAlarmCleared,
                        RollOutAlarmExcused = thisTruck.Status.RollOutAlarmExcused,
                        RollOutAlarmComments = thisTruck.Status.RollOutAlarmComments,
                        OnPatrolAlarm = thisTruck.Status.OnPatrolAlarm,
                        OnPatrolAlarmTime = thisTruck.Status.OnPatrolAlarmTime,
                        OnPatrolAlarmCleared = thisTruck.Status.OnPatrolAlarmCleared,
                        OnPatrolAlarmExcused = thisTruck.Status.OnPatrolAlarmExcused,
                        OnPatrolAlarmComments = thisTruck.Status.OnPatrolAlarmComments,
                        LogOffAlarm = thisTruck.Status.LogOffAlarm,
                        LogOffAlarmTime = thisTruck.Status.LogOffAlarmTime,
                        LogOffAlarmCleared = thisTruck.Status.LogOffAlarmCleared,
                        LogOffAlarmExcused = thisTruck.Status.LogOffAlarmExcused,
                        LogOffAlarmComments = thisTruck.Status.LogOffAlarmComments,
                        IncidentAlarm = thisTruck.Status.IncidentAlarm,
                        IncidentAlarmTime = thisTruck.Status.IncidentAlarmTime,
                        IncidentAlarmCleared = thisTruck.Status.IncidentAlarmCleared,
                        IncidentAlarmExcused = thisTruck.Status.IncidentAlarmExcused,
                        IncidentAlarmComments = thisTruck.Status.IncidentAlarmComments,
                        GPSIssueAlarm = thisTruck.Status.GPSIssueAlarm,
                        GPSIssueAlarmStart = thisTruck.Status.GPSIssueAlarmStart,
                        GPSIssueAlarmCleared = thisTruck.Status.GPSIssueAlarmCleared,
                        GPSIssueAlarmExcused = thisTruck.Status.GPSIssueAlarmExcused,
                        GPSIssueAlarmComments = thisTruck.Status.GPSIssueAlarmComments,
                        StationaryAlarm = thisTruck.Status.StationaryAlarm,
                        StationaryAlarmStart = thisTruck.Status.StationaryAlarmStart,
                        StationaryAlarmCleared = thisTruck.Status.StationaryAlarmCleared,
                        StationaryAlarmExcused = thisTruck.Status.StationaryAlarmExcused,
                        StationaryAlarmComments = thisTruck.Status.StationaryAlarmComments
                    });
                }
                return AllAlarms;
            }
            catch
            {
                return null;
            }
        }

        public List<AlarmStatus> AlarmByTruck(string IPAddress)
        {
            try
            {
                List<AlarmStatus> thisAlarmStatus = new List<AlarmStatus>();
                TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(IPAddress);
                if (thisTruck != null)
                {
                    thisAlarmStatus.Add(new AlarmStatus
                    {
                        BeatNumber = thisTruck.assignedBeat.BeatNumber.ToString(),
                        VehicleNumber = thisTruck.Extended.TruckNumber,
                        DriverName = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName,
                        ContractCompanyName = thisTruck.Extended.ContractorName,
                        SpeedingAlarm = thisTruck.Status.SpeedingAlarm,
                        SpeedingValue = thisTruck.Status.SpeedingValue,
                        SpeedingTime = thisTruck.Status.SpeedingTime,
                        OutOfBoundsAlarm = thisTruck.Status.OutOfBoundsAlarm,
                        OutOfBoundsMessage = thisTruck.Status.OutOfBoundsMessage,
                        OutOfBoundsTime = thisTruck.Status.OutOfBoundsTime,
                        OutOfBoundsStartTime = thisTruck.Status.OutOfBoundsStartTime,
                        VehicleStatus = thisTruck.Status.VehicleStatus,
                        StatusStarted = thisTruck.Status.StatusStarted,
                        LogOnAlarm = thisTruck.Status.LogOnAlarm,
                        LogOnAlarmTime = thisTruck.Status.LogOnAlarmTime,
                        LogOnAlarmCleared = thisTruck.Status.LogOnAlarmCleared,
                        LogOnAlarmExcused = thisTruck.Status.LogOnAlarmCleared,
                        LogOnAlarmComments = thisTruck.Status.LogOnAlarmComments,
                        RollInAlarm = thisTruck.Status.RollInAlarm,
                        RollInAlarmTime = thisTruck.Status.RollInAlarmTime,
                        RollInAlarmCleared = thisTruck.Status.RollInAlarmCleared,
                        RollInAlarmExcused = thisTruck.Status.RollInAlarmExcused,
                        RollInAlarmComments = thisTruck.Status.RollInAlarmComments,
                        RollOutAlarm = thisTruck.Status.RollOutAlarm,
                        RollOutAlarmTime = thisTruck.Status.RollOutAlarmTime,
                        RollOutAlarmCleared = thisTruck.Status.RollOutAlarmCleared,
                        RollOutAlarmExcused = thisTruck.Status.RollOutAlarmExcused,
                        RollOutAlarmComments = thisTruck.Status.RollOutAlarmComments,
                        OnPatrolAlarm = thisTruck.Status.OnPatrolAlarm,
                        OnPatrolAlarmTime = thisTruck.Status.OnPatrolAlarmTime,
                        OnPatrolAlarmCleared = thisTruck.Status.OnPatrolAlarmCleared,
                        OnPatrolAlarmExcused = thisTruck.Status.OnPatrolAlarmExcused,
                        OnPatrolAlarmComments = thisTruck.Status.OnPatrolAlarmComments,
                        LogOffAlarm = thisTruck.Status.LogOffAlarm,
                        LogOffAlarmTime = thisTruck.Status.LogOffAlarmTime,
                        LogOffAlarmCleared = thisTruck.Status.LogOffAlarmCleared,
                        LogOffAlarmExcused = thisTruck.Status.LogOffAlarmExcused,
                        LogOffAlarmComments = thisTruck.Status.LogOffAlarmComments,
                        IncidentAlarm = thisTruck.Status.IncidentAlarm,
                        IncidentAlarmTime = thisTruck.Status.IncidentAlarmTime,
                        IncidentAlarmCleared = thisTruck.Status.IncidentAlarmCleared,
                        IncidentAlarmExcused = thisTruck.Status.IncidentAlarmExcused,
                        IncidentAlarmComments = thisTruck.Status.IncidentAlarmComments,
                        GPSIssueAlarm = thisTruck.Status.GPSIssueAlarm,
                        GPSIssueAlarmStart = thisTruck.Status.GPSIssueAlarmStart,
                        GPSIssueAlarmCleared = thisTruck.Status.GPSIssueAlarmCleared,
                        GPSIssueAlarmExcused = thisTruck.Status.GPSIssueAlarmExcused,
                        GPSIssueAlarmComments = thisTruck.Status.GPSIssueAlarmComments,
                        StationaryAlarm = thisTruck.Status.StationaryAlarm,
                        StationaryAlarmStart = thisTruck.Status.StationaryAlarmStart,
                        StationaryAlarmCleared = thisTruck.Status.StationaryAlarmCleared,
                        StationaryAlarmExcused = thisTruck.Status.StationaryAlarmExcused,
                        StationaryAlarmComments = thisTruck.Status.StationaryAlarmComments
                    });
                }
                return thisAlarmStatus;
            }
            catch
            {
                return null;
            }
        }

        public void UnexcuseAlarm(string _vehicleNumber, string _beatNumber, string _alarm, string _driverName, string _comments = "NO COMMENT")
        {
            try
            {
                SQL.SQLCode mySQL = new SQL.SQLCode();
                TowTruck.TowTruck t = DataClasses.GlobalData.FindTowTruckByVehicleNumber(_vehicleNumber);
                if (string.IsNullOrEmpty(_comments))
                {
                    _comments = "NO COMMENT";
                }
                if (t != null)
                {
                    switch (_alarm.ToUpper())
                    {
                        case "LOGON":
                            //t.Status.LogOnAlarm = false;
                            //t.Status.LogOnAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOnAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOnAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGON", _comments, _beatNumber);
                            break;
                        case "ROLLIN":
                            //t.Status.RollInAlarm = false;
                            //t.Status.RollInAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollInAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollInAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLL IN", _comments, _beatNumber);
                            break;
                        case "ROLLOUT":
                            //t.Status.RollOutAlarm = false;
                            //t.Status.RollOutAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollOutAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollOutAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLL OUT", _comments, _beatNumber);
                            break;
                        case "ONPATROL":
                            //t.Status.OnPatrolAlarm = false;
                            //t.Status.OnPatrolAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.OnPatrolAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.OnPatrolAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ON PATROL", _comments, _beatNumber);
                            break;
                        case "LOGOFF":
                            //t.Status.LogOffAlarm = false;
                            //t.Status.LogOffAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOffAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOffAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGOFF", _comments, _beatNumber);
                            break;
                        case "INCIDENT":
                            //t.Status.IncidentAlarm = false;
                            //t.Status.IncidentAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.IncidentAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.IncidentAlarmComments = _comments;
                            t.Status.StatusStarted = DateTime.Now;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "INCIDENT", _comments, _beatNumber);
                            break;
                        case "GPSISSUE": //handles NO GPS signal: 0,0
                            //t.Status.NoMotionAlarm = false;
                            //t.Status.NoMotionAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.GPSIssueAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.GPSIssueAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "GPS ISSUE", _comments, _beatNumber);
                            //t.Status.StatusStarted = DateTime.Now;
                            break;
                        case "STATIONARY": //handles NO MOVEMENT: speed = 0
                            //t.Status.StationaryAlarm = false;
                            //t.Status.StationaryAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.StationaryAlarmExcused = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.StationaryAlarmComments = _comments;
                            mySQL.UnExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "STATIONARY", _comments, _beatNumber);
                            break;
                    }
                }
                else
                {
                    //truck isn't currently in the system, so we need to do some lookups
                    Guid truckID = mySQL.GetTruckID(_vehicleNumber);
                    string[] splitter = _driverName.Split(',');
                    Guid driverID = mySQL.GetDriverID(splitter[0].ToString().Trim(), splitter[1].ToString().Trim());
                    if (truckID != new Guid("00000000-0000-0000-0000-000000000000") && driverID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        mySQL.UnExcuseAlarm(driverID, truckID, _alarm.ToUpper(), _comments, _beatNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }

        public void ExcuseAlarm(string _vehicleNumber, string _beatNumber, string _alarm, string _driverName, string _comments = "NO COMMENT")
        {
            try
            {
                SQL.SQLCode mySQL = new SQL.SQLCode();
                TowTruck.TowTruck t = DataClasses.GlobalData.FindTowTruckByVehicleNumber(_vehicleNumber);
                if (string.IsNullOrEmpty(_comments))
                {
                    _comments = "NO COMMENT";
                }
                if (t != null)
                {
                    switch (_alarm.ToUpper())
                    {
                        case "LOGON":
                            //t.Status.LogOnAlarm = false;
                            //t.Status.LogOnAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOnAlarmExcused = DateTime.Now;
                            t.Status.LogOnAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGON", _comments, _beatNumber);
                            break;
                        case "ROLLIN":
                            //t.Status.RollInAlarm = false;
                            //t.Status.RollInAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollInAlarmExcused = DateTime.Now;
                            t.Status.RollInAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLL IN", _comments, _beatNumber);
                            break;
                        case "ROLLOUT":
                            //t.Status.RollOutAlarm = false;
                            //t.Status.RollOutAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollOutAlarmExcused = DateTime.Now;
                            t.Status.RollOutAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLL OUT", _comments, _beatNumber);
                            break;
                        case "ONPATROL":
                            //t.Status.OnPatrolAlarm = false;
                            //t.Status.OnPatrolAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.OnPatrolAlarmExcused = DateTime.Now;
                            t.Status.OnPatrolAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ON PATROL", _comments, _beatNumber);
                            break;
                        case "LOGOFF":
                            //t.Status.LogOffAlarm = false;
                            //t.Status.LogOffAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOffAlarmExcused = DateTime.Now;
                            t.Status.LogOffAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGOFF", _comments, _beatNumber);
                            break;
                        case "INCIDENT":
                            //t.Status.IncidentAlarm = false;
                            //t.Status.IncidentAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.IncidentAlarmExcused = DateTime.Now;
                            t.Status.IncidentAlarmComments = _comments;
                            t.Status.StatusStarted = DateTime.Now;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "INCIDENT", _comments, _beatNumber);
                            break;
                        case "GPSISSUE": //handles NO GPS signal: 0,0
                            //t.Status.NoMotionAlarm = false;
                            //t.Status.NoMotionAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.GPSIssueAlarmExcused = DateTime.Now;
                            t.Status.GPSIssueAlarmComments = _comments;
                            mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "GPS ISSUE", _comments, _beatNumber);
                            //t.Status.StatusStarted = DateTime.Now;
                            break;
                        case "STATIONARY": //handles NO MOVEMENT: speed = 0
                            //t.Status.StationaryAlarm = false;
                            //t.Status.StationaryAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.StationaryAlarmExcused = DateTime.Now;
                            t.Status.StationaryAlarmComments = _comments;
                            //mySQL.ExcuseAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "STATIONARY", _comments, _beatNumber);
                            break;
                    }
                }
                else
                {
                    //truck isn't currently in the system, so we need to do some lookups
                    Guid truckID = mySQL.GetTruckID(_vehicleNumber);
                    string[] splitter = _driverName.Split(',');
                    Guid driverID = mySQL.GetDriverID(splitter[0].ToString().Trim(), splitter[1].ToString().Trim());
                    if (truckID != new Guid("00000000-0000-0000-0000-000000000000") && driverID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        mySQL.ExcuseAlarm(driverID, truckID, _alarm.ToUpper(), _comments, _beatNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }

        public void ClearAlarm(string _vehicleNumber, string _alarm)
        {
            try
            {
                SQL.SQLCode mySQL = new SQL.SQLCode();
                TowTruck.TowTruck t = DataClasses.GlobalData.FindTowTruckByVehicleNumber(_vehicleNumber);
                if (t != null)
                {
                    switch (_alarm.ToUpper())
                    {
                        case "LOGON":
                            t.Status.LogOnAlarm = false;
                            t.Status.LogOnAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOnAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGON");
                            break;
                        case "ROLLIN":
                            t.Status.RollInAlarm = false;
                            t.Status.RollInAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollInAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLLIN");
                            break;
                        case "ROLLOUT":
                            t.Status.RollOutAlarm = false;
                            t.Status.RollOutAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.RollOutAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ROLLOUT");
                            break;
                        case "ONPATROL":
                            t.Status.OnPatrolAlarm = false;
                            t.Status.OnPatrolAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.OnPatrolAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "ONPATROL");
                            break;
                        case "LOGOFF":
                            t.Status.LogOffAlarm = false;
                            t.Status.LogOffAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.LogOffAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "LOGOFF");
                            break;
                        case "INCIDENT":
                            t.Status.IncidentAlarm = false;
                            t.Status.IncidentAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.IncidentAlarmCleared = DateTime.Now;
                            t.Status.StatusStarted = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "INCIDENT");
                            break;
                        case "GPSISSUE": //handles NO GPS signal: 0,0
                            t.Status.GPSIssueAlarm = false;
                            t.Status.GPSIssueAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.GPSIssueAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "GPSISSUE");
                            //t.Status.StatusStarted = DateTime.Now;
                            break;
                        case "STATIONARY": //handles NO MOVEMENT: speed = 0
                            t.Status.StationaryAlarm = false;
                            t.Status.StationaryAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.StationaryAlarmCleared = DateTime.Now;
                            mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "STATIONARY");
                            break;
                        case "OFFBEAT":
                            t.Status.OutOfBoundsAlarm = false;
                            t.Status.OutOfBoundsStartTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.OutOfBoundsTimeCleared = DateTime.Now;
                            //mySQL.ClearAlarm(t.Driver.DriverID, t.Extended.FleetVehicleID, "OFFBEAT");
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }

        public void ResetAlarm(string _vehicleNumber, string _alarm)
        {
            try
            {
                TowTruck.TowTruck t = DataClasses.GlobalData.FindTowTruckByVehicleNumber(_vehicleNumber);
                if (t != null)
                {
                    switch (_alarm.ToUpper())
                    {
                        case "LOGON":
                            t.Status.LogOnAlarm = false;
                            t.Status.LogOnAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "ROLLIN":
                            t.Status.RollInAlarm = false;
                            t.Status.RollInAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "ROLLOUT":
                            t.Status.RollOutAlarm = false;
                            t.Status.RollOutAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "ONPATROL":
                            t.Status.OnPatrolAlarm = false;
                            t.Status.OnPatrolAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "LOGOFF":
                            t.Status.LogOffAlarm = false;
                            t.Status.LogOffAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "INCIDENT":
                            t.Status.IncidentAlarm = false;
                            t.Status.IncidentAlarmTime = DateTime.Parse("01/01/2001 00:00:00");
                            t.Status.StatusStarted = DateTime.Now;
                            break;
                        case "GPSISSUE": //handles NO GPS signal: 0,0
                            t.Status.GPSIssueAlarm = false;
                            t.Status.GPSIssueAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            //t.Status.StatusStarted = DateTime.Now;
                            break;
                        case "STATIONARY": //handles NO MOVEMENT: speed = 0
                            t.Status.StationaryAlarm = false;
                            t.Status.StationaryAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                        case "OFFBEAT":
                            t.Status.OutOfBoundsAlarm = false;
                            t.Status.OutOfBoundsStartTime = DateTime.Parse("01/01/2001 00:00:00");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                string err = ex.ToString();
            }
        }

        public List<TruckMessage> GetAllMessages()
        {
            return DataClasses.GlobalData.theseMessages;
        }

        public void SendMessage(TruckMessage thisMessage)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            string UserName = mySQL.FindUserNameByID(thisMessage.UserID);
            thisMessage.MessageText += Environment.NewLine + "FROM: " + UserName;
            DataClasses.GlobalData.AddTruckMessage(thisMessage);
        }

        public List<ListDrivers> GetTruckDrivers()
        {
            List<ListDrivers> truckDrivers = new List<ListDrivers>();
            foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks)
            {
                truckDrivers.Add(new ListDrivers
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

        public void LogOffDriver(Guid DriverID)
        {
            DataClasses.GlobalData.ForceDriverLogoff(DriverID);
        }
    }
}
