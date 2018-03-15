﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.SqlServer.Types;

namespace FPSService.SQL
{
    public class SQLCode
    {
        string ConnStr;
        string ConnBeat;
        Logging.EventLogger logger;

        public SQLCode()
        {
            ConnStr = ConfigurationManager.AppSettings["FSPdb"].ToString();
            ConnBeat = ConfigurationManager.AppSettings["BeatDB"].ToString();
        }

        #region " SQL Reads "

        #region " One-time loads, execute at start of service"

        public void LoadContractors()
        {
            logger = new Logging.EventLogger();
            string SQL = "SELECT ContractorID, ContractCompanyName FROM Contractors ORDER BY ContractCompanyName";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.Contractors thisContractor = new MiscData.Contractors();
                        thisContractor.ContractorID = new Guid(rdr["ContractorID"].ToString());
                        thisContractor.ContractCompanyName = rdr["ContractCompanyName"].ToString();
                        DataClasses.GlobalData.Contractors.Add(thisContractor);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Contractors" + Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadCode1098s()
        {
            logger = new Logging.EventLogger();
            string SQL = "Get1098Codes";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        MiscData.Code1098 thisCode = new MiscData.Code1098();
                        thisCode.CodeID = new Guid(rdr["CodeID"].ToString());
                        thisCode.Code = rdr["Code"].ToString();
                        thisCode.CodeName = rdr["CodeName"].ToString();
                        thisCode.CodeDescription = rdr["CodeDescription"].ToString();
                        thisCode.CodeCall = rdr["CodeCall"].ToString();
                        DataClasses.GlobalData.Code1098s.Add(thisCode);
                    }

                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading 1098 Codes" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadFreeways()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetFreeways";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.Freeway thisFreeway = new MiscData.Freeway();
                        thisFreeway.FreewayID = Convert.ToInt32(rdr["FreewayID"]);
                        thisFreeway.FreewayName = rdr["FreewayName"].ToString();
                        DataClasses.GlobalData.Freeways.Add(thisFreeway);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Freeways" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadIncidentTypes()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetIncidentTypes";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.IncidentType thisIncidentType = new MiscData.IncidentType();
                        thisIncidentType.IncidentTypeID = new Guid(rdr["IncidentTypeID"].ToString());
                        thisIncidentType.IncidentTypeCode = rdr["IncidentTypeCode"].ToString();
                        thisIncidentType.IncidentTypeName = rdr["IncidentType"].ToString();
                        DataClasses.GlobalData.IncidentTypes.Add(thisIncidentType);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Incident Types" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadLocationCoding()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetLocationCoding";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.LocationCoding thisLocationCoding = new MiscData.LocationCoding();
                        thisLocationCoding.LocationID = new Guid(rdr["LocationID"].ToString());
                        thisLocationCoding.LocationCode = rdr["LocationCode"].ToString();
                        thisLocationCoding.Location = rdr["Location"].ToString();
                        DataClasses.GlobalData.LocationCodes.Add(thisLocationCoding);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Location Coding" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadServiceTypes()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetServiceTypes";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.ServiceType thisServiceType = new MiscData.ServiceType();
                        thisServiceType.ServiceTypeID = new Guid(rdr["ServiceTypeID"].ToString());
                        thisServiceType.ServiceTypeCode = rdr["ServiceTypeCode"].ToString();
                        thisServiceType.ServiceTypeName = rdr["ServiceType"].ToString();
                        DataClasses.GlobalData.ServiceTypes.Add(thisServiceType);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Service Types" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadTowLocations()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetTowLocations";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.TowLocation thisTowLocation = new MiscData.TowLocation();
                        thisTowLocation.TowLocationID = new Guid(rdr["TowLocationID"].ToString());
                        thisTowLocation.TowLocationCode = rdr["TowLocationCode"].ToString();
                        thisTowLocation.TowLocationName = rdr["TowLocation"].ToString();
                        DataClasses.GlobalData.TowLocations.Add(thisTowLocation);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Tow Locations" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadTrafficSpeeds()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetTrafficSpeeds";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.TrafficSpeed thisTrafficSpeed = new MiscData.TrafficSpeed();
                        thisTrafficSpeed.TrafficSpeedID = new Guid(rdr["TrafficSpeedID"].ToString());
                        thisTrafficSpeed.TrafficSpeedCode = rdr["TrafficSpeedCode"].ToString();
                        thisTrafficSpeed.TrafficSpeedName = rdr["TrafficSpeed"].ToString();
                        DataClasses.GlobalData.TrafficSpeeds.Add(thisTrafficSpeed);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Traffic Speeds" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadVehiclePositions()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetVehiclePositions";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.VehiclePosition thisVehiclePosition = new MiscData.VehiclePosition();
                        thisVehiclePosition.VehiclePositionID = new Guid(rdr["VehiclePositionID"].ToString());
                        thisVehiclePosition.VehiclePositionCode = rdr["VehiclePositionCode"].ToString();
                        thisVehiclePosition.VehiclePositionName = rdr["VehiclePosition"].ToString();
                        DataClasses.GlobalData.VehiclePositions.Add(thisVehiclePosition);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Vehicle Positions" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadVehicleTypes()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetVehicleTypes";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.VehicleType thisVehicleType = new MiscData.VehicleType();
                        thisVehicleType.VehicleTypeID = new Guid(rdr["VehicleTypeID"].ToString());
                        thisVehicleType.VehicleTypeCode = rdr["VehicleTypeCode"].ToString();
                        thisVehicleType.VehicleTypeName = rdr["VehicleType"].ToString();
                        DataClasses.GlobalData.VehicleTypes.Add(thisVehicleType);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Vehicle Types" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadDropZones()
        {
            logger = new Logging.EventLogger();
            string SQL = "GetDropZones";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MiscData.DropZone thisDropZone = new MiscData.DropZone();
                        thisDropZone.DropZoneID = new Guid(rdr["DropZoneID"].ToString());
                        thisDropZone.Location = rdr["Location"].ToString();
                        thisDropZone.Comments = rdr["Comments"].ToString();
                        thisDropZone.Restrictions = rdr["Restrictions"].ToString();
                        thisDropZone.DropZoneNumber = rdr["DropZoneNumber"].ToString();
                        thisDropZone.DropZoneDescription = rdr["DropZoneDescription"].ToString();
                        thisDropZone.City = rdr["City"].ToString();
                        thisDropZone.PDPhoneNumber = rdr["PDPhoneNumber"].ToString();
                        thisDropZone.Capacity = Convert.ToInt32(rdr["Capacity"]);
                        thisDropZone.Position = SqlGeography.Deserialize(rdr.GetSqlBytes(9));
                        DataClasses.GlobalData.DropZones.Add(thisDropZone);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Drop Zones" +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LoadLeeways()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "SELECT VarName, VarValue FROM Vars WHERE VarName LIKE '%Leeway'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        int LeewayVal = Convert.ToInt32(rdr["VarValue"]);
                        switch(rdr["VarName"].ToString())
                        {
                            case "RollOutLeeway":
                                DataClasses.GlobalData.RollOutLeeway = LeewayVal;
                                break;
                            case "StationaryLeeway":
                                DataClasses.GlobalData.StationaryLeeway = LeewayVal;
                                break;
                            case "LogOnLeeway":
                                DataClasses.GlobalData.LogOnLeeway = LeewayVal;
                                break;
                            case "OnPatrolLeeway":
                                DataClasses.GlobalData.OnPatrollLeeway = LeewayVal;
                                break;
                            case "RollInLeeway":
                                DataClasses.GlobalData.RollInLeeway = LeewayVal;
                                break;
                            case "LogOffLeeway":
                                DataClasses.GlobalData.LogOffLeeway = LeewayVal;
                                break;
                            case "SpeedingLeeway":
                                DataClasses.GlobalData.SpeedingLeeway = LeewayVal;
                                break;
                            case "OffBeatLeeway":
                                DataClasses.GlobalData.OffBeatLeeway = LeewayVal;
                                break;
                            case "ExtendedLeeway":
                                DataClasses.GlobalData.ExtendedLeeway = LeewayVal;
                                break;
                            case "GPSIssueLeeway":
                                DataClasses.GlobalData.GPSIssueLeeway = LeewayVal;
                                break;
                            case "ForcedOffLeeway":
                                DataClasses.GlobalData.ForceOff = LeewayVal;
                                break;
                        }
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Leeways" + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void LoadBeatSchedules()
        {
            //First check: are there schedules for the day?
            //Second check: what are those schedules?
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    DataClasses.GlobalData.theseSchedules.Clear();
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetBeatSchedules", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    string cDate = DateTime.Now.ToShortDateString();
                    while (rdr.Read())
                    {
                        MiscData.BeatSchedule thisSchedule = new MiscData.BeatSchedule();
                        thisSchedule.BeatID = new Guid(rdr["BeatID"].ToString());
                        thisSchedule.BeatScheduleID = new Guid(rdr["BeatScheduleID"].ToString());
                        thisSchedule.BeatNumber = rdr["BeatNumber"].ToString();
                        thisSchedule.ScheduleName = rdr["ScheduleName"].ToString();
                        bool Weekday = false;
                        string wdtest = rdr["Weekday"].ToString();
                        if(rdr["Weekday"].ToString() == "True")
                        {
                            Weekday = true;
                        }
                        thisSchedule.Weekday = Weekday;
                        thisSchedule.Logon = Convert.ToDateTime(cDate + " " + rdr["Logon"]);
                        thisSchedule.RollOut = Convert.ToDateTime(cDate + " " + rdr["RollOut"]);
                        thisSchedule.OnPatrol = Convert.ToDateTime(cDate + " " + rdr["OnPatrol"]);
                        thisSchedule.RollIn = Convert.ToDateTime(cDate + " " + rdr["RollIn"]);
                        thisSchedule.LogOff = Convert.ToDateTime(cDate + " " + rdr["LogOff"]);
                        DataClasses.GlobalData.AddBeatSchedule(thisSchedule);
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR Loading Beat Schedules" + Environment.NewLine + ex.ToString(), true);
            }
        }

        #endregion

        public Guid GetDriverID(string _lastName, string _firstName)
        {
            Guid gDriver = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "SELECT DriverID FROM Drivers WHERE LastName = '" + _lastName + "' AND FirstName = '" + _firstName + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    object driverIDObj = cmd.ExecuteScalar();
                    string driverIDString = "00000000-0000-0000-0000-000000000000";
                    if (driverIDObj != null)
                    {
                        driverIDString = driverIDObj.ToString();
                    }
                    //string driverIDString = cmd.ExecuteScalar().ToString();
                    gDriver = new Guid(driverIDString);
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting DriverID for " + _firstName + " " + _lastName  +
                Environment.NewLine + ex.ToString(), true);
            }
            return gDriver;
        }

        public Guid GetTruckID(string truckNumber)
        {
            Guid gTruck = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "SELECT ISNULL(FleetVehicleID, '00000000-0000-0000-0000-000000000000') FROM FleetVehicles WHERE VehicleNumber = '" + truckNumber + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    object truckIDString = cmd.ExecuteScalar();
                    cmd = null;
                    if (truckIDString != null)
                    {
                        gTruck = new Guid(truckIDString.ToString());
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting TruckID for " + truckNumber +
                               Environment.NewLine + ex.ToString(), true);
            }
            return gTruck;
        }

        public string GetMACAddress(string ID)
        {
            string MAC = "NA";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "SELECT IPAddress FROM FleetVehicles WHERE VehicleNumber = '" + ID + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    object truckIP = cmd.ExecuteScalar();
                    if (truckIP != null)
                    {
                        MAC = truckIP.ToString();
                    }
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting IPAddress for " + ID +
                               Environment.NewLine + ex.ToString(), true);
            }
            return MAC;
        }

        public string GetTruckNumberByID(Guid FleetVehicleID)
        {
            string TruckNumber = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetVehicleNumber", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FleetVehicleID", FleetVehicleID);
                    TruckNumber = cmd.ExecuteScalar().ToString();
                    cmd = null;
                    conn.Close();
                }
               
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Next Survey Number " +
                Environment.NewLine + ex.ToString(), true);
            }
            return TruckNumber;
        }

        public int GetUsedBreakTime(string DriverID, string Type)
        {
            int UsedBreakTime = 0;
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("FindUsedBreakTime", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BreakType", Type);
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    var returnVal = cmd.ExecuteScalar();
                    if (returnVal != null)
                    {
                        UsedBreakTime = Convert.ToInt32(returnVal);
                        if (UsedBreakTime < 0)
                        {
                            UsedBreakTime = 0;
                        }
                    }
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Used BreakTime: " +
                 Environment.NewLine + ex.ToString(), true);
            }
            return UsedBreakTime;
        }

        public string GetVarValue(string VarName)
        {
            string VarValue;
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "SELECT VarValue FROM Vars WHERE VarName = '" + VarName + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    VarValue = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Next Incident Number " +
                  Environment.NewLine + ex.ToString(), true);
                VarValue = "error";
            }
            return VarValue;
        }

        public string GetSurveyNum()
        {
            logger = new Logging.EventLogger();
            string SurveyNum = "NA";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    int TodayNum = 0;
                    int DBNum = 0;
                    int SurveyNumVal = 0;
                    string sMonth = DateTime.Now.Month.ToString();
                    string sDay = DateTime.Now.Day.ToString();
                    string sYear = DateTime.Now.Year.ToString();
                    while (sMonth.Length < 2)
                    {
                        sMonth = "0" + sMonth;
                    }
                    while (sDay.Length < 2)
                    {
                        sDay = "0" + sDay;
                    }
                    while (sYear.Length < 4)
                    {
                        sYear = "0" + sYear;
                    }
                    SurveyNum = sMonth + sDay + sYear + "SN";
                    string SQL = "SELECT DATEPART(dy,GETDATE())"; //get TodayNum
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    TodayNum = Convert.ToInt32(cmd.ExecuteScalar());

                    SQL = "SELECT VarValue FROM Vars WHERE VarName = 'SurveyNumLastUpdate'"; //get DBNum
                    cmd = new SqlCommand(SQL, conn);
                    DBNum = Convert.ToInt32(cmd.ExecuteScalar());

                    if (TodayNum != DBNum)
                    {
                        cmd = new SqlCommand("ResetSurveyNums", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }

                    cmd = new SqlCommand("GetNextSurveyNum", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    string NextSNum = Convert.ToString(cmd.ExecuteScalar());
                    SurveyNum += NextSNum;

                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Next Survey Number " +
                      Environment.NewLine + ex.ToString(), true);
                    if (conn.State == ConnectionState.Open)
                    { conn.Close(); }
                }
            }
            return SurveyNum;
        }

        public int GetNextIncidentNumber()
        {
            logger = new Logging.EventLogger();
            int NextNumber = 2147483647;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    string SQL = "GetIncidentCountForDay";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    NextNumber = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Next Incident Number " + 
                       Environment.NewLine + ex.ToString(), true);
                    if (conn.State == ConnectionState.Open)
                    { conn.Close(); }
                }
                return NextNumber;
            }
        }

        public TowTruck.TowTruckExtended GetExtendedData(string IPAddress)
        {
            logger = new Logging.EventLogger();
            TowTruck.TowTruckExtended thisTruckExtended = null;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    string SQL = "GetExtendedData";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    thisTruckExtended = new TowTruck.TowTruckExtended();

                    while (rdr.Read())
                    {

                        thisTruckExtended.ContractorName = rdr["ContractCompanyName"].ToString();
                        thisTruckExtended.FleetNumber = rdr["FleetNumber"].ToString();
                        thisTruckExtended.ProgramStartDate = Convert.ToDateTime(rdr["ProgramStartDate"]);
                        thisTruckExtended.VehicleType = rdr["VehicleType"].ToString();
                        thisTruckExtended.VehicleYear = Convert.ToInt32(rdr["VehicleYear"]);
                        thisTruckExtended.VehicleMake = rdr["VehicleMake"].ToString();
                        thisTruckExtended.VehicleModel = rdr["VehicleModel"].ToString();
                        thisTruckExtended.LicensePlate = rdr["LicensePlate"].ToString();
                        thisTruckExtended.RegistrationExpireDate = Convert.ToDateTime(rdr["RegistrationExpireDate"]);
                        thisTruckExtended.InsuranceExpireDate = Convert.ToDateTime(rdr["InsuranceExpireDate"]);
                        thisTruckExtended.LastCHPInspection = Convert.ToDateTime(rdr["LastCHPInspection"]);
                        thisTruckExtended.ProgramEndDate = Convert.ToDateTime(rdr["ProgramEndDate"]);
                        thisTruckExtended.FAW = Convert.ToInt32(rdr["FAW"]);
                        thisTruckExtended.RAW = Convert.ToInt32(rdr["RAW"]);
                        thisTruckExtended.RAWR = Convert.ToInt32(rdr["RAWR"]);
                        thisTruckExtended.GVW = Convert.ToInt32(rdr["GVW"]);
                        thisTruckExtended.GVWR = Convert.ToInt32(rdr["GVWR"]);
                        thisTruckExtended.Wheelbase = Convert.ToInt32(rdr["Wheelbase"]);
                        thisTruckExtended.Overhang = Convert.ToInt32(rdr["Overhang"]);
                        thisTruckExtended.MAXTW = Convert.ToInt32(rdr["MAXTW"]);
                        thisTruckExtended.MAXTWCALCDATE = Convert.ToDateTime(rdr["MAXTWCALCDATE"]);
                        thisTruckExtended.FuelType = rdr["FuelType"].ToString();
                        thisTruckExtended.TruckNumber = rdr["VehicleNumber"].ToString();
                        thisTruckExtended.FleetVehicleID = new Guid(rdr["FleetVehicleID"].ToString());
                        thisTruckExtended.ContractorID = new Guid(rdr["ContractorID"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Extended Data for Tow Truck " + IPAddress +
                        Environment.NewLine + ex.ToString(), true);
                    if (conn.State == ConnectionState.Open)
                    { conn.Close(); }
                }
            }
            return thisTruckExtended;
        }

        public List<BeatData.Yard> LoadYards()
        {
            logger = new Logging.EventLogger();
            List<BeatData.Yard> theseYards = new List<BeatData.Yard>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnBeat))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetAllYards", conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        BeatData.Yard thisYard = new BeatData.Yard();
                        thisYard.YardID = new Guid(rdr["TowTruckYardID"].ToString());
                        thisYard.Location = rdr["Location"].ToString();
                        thisYard.TowTruckCompanyName = rdr["TowTruckCompanyName"].ToString();
                        thisYard.Position = SqlGeography.Deserialize(rdr.GetSqlBytes(3));
                        thisYard.YardDescription = rdr["TowTruckYardDescription"].ToString();
                        theseYards.Add(thisYard);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error loading yards" + Environment.NewLine + ex.ToString(), true);
            }
            return theseYards;
        }

        public List<BeatData.Beat> LoadBeatsOnly()
        {
            logger = new Logging.EventLogger();
            List<BeatData.Beat> theseBeats = new List<BeatData.Beat>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnBeat))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LoadBeatsOnly", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        BeatData.Beat thisBeat = new BeatData.Beat();
                        thisBeat.BeatID = new Guid(rdr["BeatID"].ToString());
                        thisBeat.BeatDescription = rdr["BeatDescription"].ToString();
                        thisBeat.BeatExtent = SqlGeography.Deserialize(rdr.GetSqlBytes(2));
                        thisBeat.FreewayID = Convert.ToInt32(rdr["FreewayID"]);
                        thisBeat.BeatNumber = rdr["BeatNumber"].ToString();
                        thisBeat.IsTemporary = Convert.ToBoolean(rdr["IsTemporary"]);
                        theseBeats.Add(thisBeat);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Beats Only" + Environment.NewLine + ex.ToString(), true);
            }
            if (theseBeats.Count > 0)
            { return theseBeats; }
            else
            { return null; }
        }

        public List<BeatData.BeatSegment> LoadSegmentsOnly()
        {
            logger = new Logging.EventLogger();
            List<BeatData.BeatSegment> theseSegments = new List<BeatData.BeatSegment>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnBeat))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LoadBeatSegmentsOnly", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        BeatData.BeatSegment thisSegment = new BeatData.BeatSegment();
                        thisSegment.BeatSegmentID = new Guid(rdr["BeatSegmentID"].ToString());
                        thisSegment.CHPDescription = rdr["CHPDescription"].ToString();
                        thisSegment.BeatSegmentExtent = SqlGeography.Deserialize(rdr.GetSqlBytes(2));
                        thisSegment.BeatSegmentNumber = rdr["BeatSegmentNumber"].ToString();
                        thisSegment.BeatSegmentDescription = rdr["BeatSegmentDescription"].ToString();
                        thisSegment.BeatID = new Guid(rdr["BeatID"].ToString());
                        theseSegments.Add(thisSegment);
                    }
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Loading Beat Segments Only" + Environment.NewLine + ex.ToString(), true);
            }
            if (theseSegments.Count > 0)
            { return theseSegments; }
            else
            { return null; }
        }

        public string FindUserNameByID(Guid ID)
        {
            logger = new Logging.EventLogger();
            string UserName = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("FindUserName", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", ID);
                    UserName = Convert.ToString(cmd.ExecuteScalar());
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Finding User Name by ID" + Environment.NewLine + ex.ToString(), true);
            }
            if (string.IsNullOrEmpty(UserName))
            {
                return "Unknown";
            }
            else
            {
                return UserName;
            }
        }

        public string FindDriverNameByID(Guid ID)
        {
            logger = new Logging.EventLogger();
            string DriverName = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("FindDriverNameByID", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", ID.ToString());
                    DriverName = Convert.ToString(cmd.ExecuteScalar());
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Finding Driver Name by ID" + Environment.NewLine + ex.ToString(), true);
            }
            if (string.IsNullOrEmpty(DriverName))
            {
                return "Unknown";
            }
            else
            {
                return DriverName;
            }
        }

        /*
        public List<BeatData.BeatClass> LoadBeats()
        {
            List<BeatData.BeatClass> theseBeats = new List<BeatData.BeatClass>();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("LoadBeats", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BeatData.BeatClass thisBeat = new BeatData.BeatClass();
                    thisBeat.BeatID = new Guid(rdr["BeatID"].ToString());
                    thisBeat.BeatDescription = rdr["BeatDescription"].ToString();
                    thisBeat.Active = Convert.ToBoolean(rdr["Active"]);
                    //thisBeat.BeatExtent = rdr.GetValue(3) as SqlGeography;
                    thisBeat.BeatExtent = SqlGeography.Deserialize(rdr.GetSqlBytes(3));
                    thisBeat.FreewayName = rdr["FreewayName"].ToString();
                    thisBeat.BeatSegmentID = new Guid(rdr["BeatSegmentID"].ToString());
                    thisBeat.BeatSegmentDescription = rdr["BeatSegmentDescription"].ToString();
                    thisBeat.BeatSegmentExtent = rdr.GetValue(7) as SqlGeography;
                    thisBeat.CHPDescription = rdr["CHPDescription"].ToString();
                    thisBeat.PIMSID = rdr["PIMSID"].ToString();
                    theseBeats.Add(thisBeat);
                }
                conn.Close();
            }
            if (theseBeats.Count > 0)
            { return theseBeats; }
            else
            { return null; }
        }
        */
        #endregion

        #region " SQL Writes "

        public void ClearAlarm(Guid DriverID, Guid VehicleID, string AlarmType)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("ClearAlarm", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    cmd.Parameters.AddWithValue("@VehicleID", VehicleID.ToString());
                    cmd.Parameters.AddWithValue("@AlarmType", AlarmType);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR CLEARING ALARM: " + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        public string GetUserName(Guid UserID)
        {
            logger = new Logging.EventLogger();
            string UserName = "Unknown User";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    string SQL = "SELECT FirstName FROM Users WHERE UserID = '" + UserID + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    object retVal = cmd.ExecuteScalar();
                    if (retVal != DBNull.Value && retVal != null)
                    {
                        UserName = cmd.ExecuteScalar().ToString();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR FINDING UserName: " + Environment.NewLine +
                                   ex.ToString(), true);
            }
            return UserName;
        }

        public void UnExcuseAlarm(Guid DriverID, Guid VehicleID, string AlarmType, string Comments, string BeatNumber)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UnExcuseAlarm", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    cmd.Parameters.AddWithValue("@VehicleID", VehicleID.ToString());
                    cmd.Parameters.AddWithValue("@AlarmType", AlarmType);
                    cmd.Parameters.AddWithValue("@Comments", Comments);
                    cmd.Parameters.AddWithValue("@BeatNumber", BeatNumber);
                    cmd.ExecuteNonQuery();
                    cmd = null;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR UNEXCUSING ALARM: " + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        public void ExcuseAlarm(Guid DriverID, Guid VehicleID, string AlarmType, string Comments, string BeatNumber)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("ExcuseAlarm", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    cmd.Parameters.AddWithValue("@VehicleID", VehicleID.ToString());
                    cmd.Parameters.AddWithValue("@AlarmType", AlarmType);
                    cmd.Parameters.AddWithValue("@Comments", Comments);
                    cmd.Parameters.AddWithValue("@BeatNumber", BeatNumber);
                    cmd.ExecuteNonQuery();
                    cmd = null;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR EXCUSING ALARM: " + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        public void LogTruckMessage(TruckMessage thisMessage)
        {
            logger = new Logging.EventLogger();
            string TruckNumber = "NA";
            string BeatNumber = "NA";
            string DriverName = "NA";
            TowTruck.TowTruck t = DataClasses.GlobalData.FindTowTruck(thisMessage.TruckIP);
            if (t != null)
            {
                TruckNumber = t.TruckNumber;
                BeatNumber = t.assignedBeat.BeatNumber;
                DriverName = t.Driver.FirstName + " " + t.Driver.FirstName;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogTruckMessage", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MessageID", thisMessage.MessageID.ToString());
                    cmd.Parameters.AddWithValue("@TruckIP", thisMessage.TruckIP.ToString());
                    cmd.Parameters.AddWithValue("@MessageText", thisMessage.MessageText);
                    cmd.Parameters.AddWithValue("@SentTime", thisMessage.SentTime.ToString());
                    cmd.Parameters.AddWithValue("@UserID", thisMessage.UserID.ToString());
                    cmd.Parameters.AddWithValue("@TruckNumber", TruckNumber);
                    cmd.Parameters.AddWithValue("@BeatNumber", BeatNumber);
                    cmd.Parameters.AddWithValue("@DriverName", DriverName);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR LOGGING TRUCK MESSAGE: " + thisMessage.MessageText + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        public void AckTruckMessage(TruckMessage thisMessage)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("AckTruckMessage", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MessageID", thisMessage.MessageID.ToString());
                    cmd.Parameters.AddWithValue("@AckTime", thisMessage.AckedTime.ToString());
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR ACKING TRUCK MESSAGE: " + thisMessage.MessageID.ToString() + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        public void LogAlarm(string AlarmType, DateTime AlarmTime, Guid DriverID, Guid VehicleID, Guid BeatID)
        {
            logger = new Logging.EventLogger();
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogAlarm", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AlarmType", AlarmType);
                    cmd.Parameters.AddWithValue("@AlarmTime", AlarmTime.ToString());
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    cmd.Parameters.AddWithValue("@VehicleID", VehicleID.ToString());
                    cmd.Parameters.AddWithValue("@BeatID", BeatID.ToString());
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + "Error logging alarm" + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void SetBreakTime(Guid DriverID, string Type, int TotalMinutes)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UpdateBreaks", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BreakType", Type);
                    cmd.Parameters.AddWithValue("@additionalMinutes", TotalMinutes);
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + "Error writing break time" + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void SetVarValue(string VarName, string VarValue)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string SQL = "UPDATE Vars SET VarValue = '" + VarValue + "' WHERE VarName = '" + VarName + "'";
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + "Error writing " + VarName + " value to " + VarValue + Environment.NewLine + ex.ToString(), true);
            }
        }

        public void LogGPS(string SQL, ArrayList arrParams)
        {
            logger = new Logging.EventLogger();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        for (int i = 0; i < arrParams.Count; i++)
                        {
                            string[] splitter = arrParams[i].ToString().Split('^');
                            cmd.Parameters.AddWithValue(splitter[0].ToString(), splitter[1].ToString());
                        }
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        string paramList = string.Empty;
                        for (int i = 0; i < arrParams.Count; i++)
                        {
                            paramList += arrParams[i].ToString() + Environment.NewLine;
                        }
                       logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + ex.ToString() + Environment.NewLine + paramList, true);
                       if (conn.State == ConnectionState.Open)
                       {conn.Close();}
                    }
                }
            }

        public void LogStatusChange(string DriverName, string VehicleID, string Status, DateTime TimeStamp, string BeatNumber)
        {
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("LogStatus", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverName", DriverName);
                    cmd.Parameters.AddWithValue("@VehicleID", VehicleID);
                    cmd.Parameters.AddWithValue("@Status", Status);
                    cmd.Parameters.AddWithValue("@TimeStamp", TimeStamp);
                    cmd.Parameters.AddWithValue("@BeatNumber", BeatNumber);
                    cmd.ExecuteNonQuery();
                    cmd = null;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Logging Off Status Change " + DriverName +
                    " " + VehicleID + " " + Status + " " + TimeStamp.ToString() +
                       Environment.NewLine + ex.ToString(), true);
            }
        }

        public void LogOffBeat(Guid DriverID, string VehicleNumber, DateTime OffBeatTime, SqlGeography location)
        {
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogOffBeatAlert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    cmd.Parameters.AddWithValue("@VehicleNumber", VehicleNumber);
                    cmd.Parameters.AddWithValue("@OffBeatTime", OffBeatTime);
                    cmd.Parameters.AddWithValue("@Location", location.ToString());
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd = null;
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Logging Off Beat " + VehicleNumber +
                       Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LogSpeeding(Guid DriverID, string VehicleNumber, double LoggedSpeed, double MaxSpeed, DateTime speedingTime, SqlGeography location)
        {
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogSpeedingAlert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    cmd.Parameters.AddWithValue("@VehicleNumber", VehicleNumber);
                    cmd.Parameters.AddWithValue("@LoggedSpeed", LoggedSpeed);
                    cmd.Parameters.AddWithValue("@MaxSpeed", MaxSpeed);
                    cmd.Parameters.AddWithValue("@SpeedingTime", speedingTime);
                    cmd.Parameters.AddWithValue("@Location", location.ToString());
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd = null;
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Logging Speeding " + VehicleNumber + 
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void FixTruckNumber(string IPAddress, string TruckNumber, Guid ContractorID)
        {
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("FixTruckNumber", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                    cmd.Parameters.AddWithValue("@VehicleNumber", TruckNumber);
                    cmd.Parameters.AddWithValue("@ContractorID", ContractorID);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Fixing Truck Number " + TruckNumber + " IP Address: " + IPAddress +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LogDriverEntry(string driverName, string truckNumber, string title, string details, string Mac)
        {
            logger = new Logging.EventLogger();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("PostDriverAppLogEntry", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverName", driverName);
                    cmd.Parameters.AddWithValue("@TruckNumber", truckNumber);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Details", details);
                    cmd.Parameters.AddWithValue("@Mac", Mac);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "ERROR LOGGING DRIVER LOG " + Environment.NewLine +
                    ex.ToString(), true);
            }
        }

        #endregion

        #region " SQL Stored Procs "

        public void RunProc(string ProcName, ArrayList arrParams)
        {
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(ProcName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams.Count > 0)
                    {
                        for (int i = 0; i < arrParams.Count; i++)
                        {
                            string[] splitter = arrParams[i].ToString().Split('~');
                            cmd.Parameters.AddWithValue(splitter[0].ToString(), splitter[1].ToString());
                        }
                    }
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    string Params = "";
                    if(arrParams.Count > 0)
                    {
                        for(int i = 0;i<arrParams.Count;i++)
                        {
                            Params += arrParams[i].ToString() + Environment.NewLine;
                        }
                    }
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error running proc: " + ProcName + " Params:" + Params +
                        Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LogEarlyRollIn(Guid DriverID, string _reason, Guid BeatID, string _vehicleID, DateTime timeStamp, string CHPLogNumber, string _exceptionType)
        {
            //this was originally specced to handle only early roll ins and was later adapted to handle mistimed events in general
            //we support late log on, roll out, on patrol, early roll in, and early log off.  All functionality works essentially the
            //same and gets logged into the EarlyRollIns table in the fsp database.
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogEarlyRollIn", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    cmd.Parameters.AddWithValue("@Reason", _reason);
                    cmd.Parameters.AddWithValue("@BeatID", BeatID.ToString());
                    cmd.Parameters.AddWithValue("@VehicleID", _vehicleID);
                    cmd.Parameters.AddWithValue("@timeStamp", timeStamp);
                    cmd.Parameters.AddWithValue("@CHPLogNumber", CHPLogNumber);
                    cmd.Parameters.AddWithValue("@ExceptionType", _exceptionType);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error logging Early Login for Driver: " + DriverID.ToString() + Environment.NewLine +
                        "Reason: " + _reason + Environment.NewLine + Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void LogEvent(Guid DriverID, string EventType)
        {
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try 
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("LogDriverEvent", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    cmd.Parameters.AddWithValue("@EventType", EventType);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "LogEvent Error" + Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public DateTime GetLastEventTime(string DriverID, string EventType)
        {
            logger = new Logging.EventLogger();
            DateTime lastTime = Convert.ToDateTime("01/01/2001 00:00:00");

            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetEventTime", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EventType", EventType);
                    cmd.Parameters.AddWithValue("@DriverID", DriverID);
                    lastTime = Convert.ToDateTime(cmd.ExecuteScalar());
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "GetLastEventTime Error" + Environment.NewLine + ex.ToString(), true);
                }
            }

            return lastTime;
        }

        public int GetBreakDuration(string DriverID)
        {
            logger = new Logging.EventLogger();
            int BreakDuration = 0;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try 
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetBreakDuration", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DriverID", DriverID.ToString());
                    BreakDuration = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd = null;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "GetBreakDuration Error" + Environment.NewLine + ex.ToString(), true);
                    return 0;
                }
            }
            return BreakDuration;
        }

        public bool CheckLogon(string IPAddr, string FSPID, string Password)
        {
            bool valid = false;
            logger = new Logging.EventLogger();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("CheckLogon", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FSPID", FSPID);
                    cmd.Parameters.AddWithValue("@password", Password);
                    string Output = Convert.ToString(cmd.ExecuteScalar());
                    if (Output == "1")
                    { valid = true; }
                    else
                    { 
                        valid = false;
                        logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "An invalid logon was attempted from " + IPAddr, true);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "CheckLogon Error" + Environment.NewLine + ex.ToString(), true);
                }

            }
            return valid;
        }

        public TowTruck.AssignedBeat GetAssignedBeat(Guid VehicleID)
        {
            TowTruck.AssignedBeat thisAssignedBeat = new TowTruck.AssignedBeat();
            logger = new Logging.EventLogger();
            thisAssignedBeat.BeatID = new Guid("00000000-0000-0000-0000-000000000000");
            thisAssignedBeat.Loaded = true;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetVehicleAssignedBeat", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FleetVehicleID", VehicleID.ToString());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        thisAssignedBeat.BeatID = new Guid(rdr["BeatID"].ToString());
                        thisAssignedBeat.BeatExtent = SqlGeography.Deserialize(rdr.GetSqlBytes(1));
                        thisAssignedBeat.BeatNumber = rdr["BeatNumber"].ToString();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            { logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Assigned Beat" + Environment.NewLine + ex.ToString(), true); }
            return thisAssignedBeat;
        }

        public TowTruck.TowTruckDriver GetDriver(string FSPID)
        {
            TowTruck.TowTruckDriver thisDriver = new TowTruck.TowTruckDriver();
            logger = new Logging.EventLogger();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetDriver", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FSPIDNumber", FSPID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        thisDriver.DriverID = new Guid(rdr["DriverID"].ToString());
                        thisDriver.LastName = rdr["LastName"].ToString();
                        thisDriver.FirstName = rdr["FirstName"].ToString();
                        thisDriver.TowTruckCompany = rdr["ContractCompanyName"].ToString();
                        thisDriver.AssignedBeat = new Guid(rdr["AssignedBeat"].ToString());
                        thisDriver.BeatScheduleID = new Guid(rdr["BeatScheduleID"].ToString());
                    }
                    rdr.Close();
                    rdr = null;
                    cmd = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            { logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error Getting Driver" + Environment.NewLine + ex.ToString(), true); }
            return thisDriver;
        }

        #endregion
    }
}