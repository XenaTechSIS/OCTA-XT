using FPSService.MiscData;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace FPSService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITowTruckService" in both code and config file together.

    #region  " Contract "

    [ServiceContract]
    public interface ITowTruckService
    {
        [OperationContract]
        List<TowTruckData> CurrentTrucks();

        [OperationContract]
        void SingleTruckDump(CopyTruck t);

        [OperationContract]
        void UnexcuseAlarm(string _vehicleNumber, string _beatNumber, string _alarm, string _driverName,
            string _comments = "NO COMMENT");

        //[OperationContract]
        //void AddTruckAssistRequest(string IPAddress, AssistReq thisReq, Guid IncidentID);

        //[OperationContract]
        //void ClearTruckAssistRequest(string IPAddress, Guid AssistRequestID);
        [OperationContract]
        void TruckDump(List<CopyTruck> trucks);

        [OperationContract]
        void ClearAlarm(string _vehicleNumber, string _alarm);

        [OperationContract]
        void ExcuseAlarm(string _vehicleNumber, string _beatNumber, string _alarm, string _driverName,
            string _comments = "NO COMMENT");

        [OperationContract]
        string[] GetPreloadedData(string Type);

        [OperationContract]
        void AddIncident(IncidentIn thisIncidentIn);

        [OperationContract]
        List<IncidentScreenData> GetAllIncidents();

        [OperationContract]
        IncidentIn FindIncidentByID(Guid IncidentID);

        [OperationContract]
        void ClearIncident(Guid IncidentID);

        [OperationContract]
        void AddAssist(AssistReq thisReq);

        [OperationContract]
        List<AssistScreenData> GetAllAssists();

        [OperationContract]
        List<AssistTruck> GetAssistTrucks();

        [OperationContract]
        int GetUsedBreakTime(string DriverID, string Type);

        [OperationContract]
        List<ListDrivers> GetTruckDrivers();

        [OperationContract]
        void LogOffDriver(Guid DriverID);

        [OperationContract]
        List<AlarmStatus> GetAllAlarms();

        [OperationContract]
        List<AlarmStatus> AlarmByTruck(string IPAddress);

        [OperationContract]
        void SendMessage(TruckMessage thisMessage);

        [OperationContract]
        List<IncidentDisplay> GetIncidentData();

        [OperationContract]
        void ResetAlarm(string _vehicleNumber, string _alarm);

        [OperationContract]
        List<BeatSegment_New> RetreiveAllSegments();

        [OperationContract]
        BeatSegment_New RetrieveSegment(Guid SegmentID);

        [OperationContract]
        string CreateSegment(BeatSegment_New segment);

        [OperationContract]
        string UpdateSegment(BeatSegment_New segment);

        [OperationContract]
        string DeleteSegment(Guid segmentid);

        [OperationContract]
        List<Beats_New> RetreiveAllBeats();

        [OperationContract]
        Beats_New RetreiveBeat(Guid BeatID);

        [OperationContract]
        string CreateBeat(Beats_New beat);

        [OperationContract]
        string UpdateBeat(Beats_New beat);

        [OperationContract]
        string UpdateBeat2(Beats_New2 beat);

        [OperationContract]
        string DeleteBeat(Guid BeatID);

        [OperationContract]
        List<Yard_New> RetreiveAllYards();

        [OperationContract]
        Yard_New RetreiveYard(Guid TowTruckYardID);

        [OperationContract]
        string CreateYard(Yard_New Yard);

        [OperationContract]
        string UpdateYard(Yard_New Yard);

        [OperationContract]
        string DeleteYard(Guid YardID);

        [OperationContract]
        List<DropZone_New> RetreiveAllDZs();

        [OperationContract]
        DropZone_New RetreiveDZ(Guid DropZoneID);

        [OperationContract]
        string CreateDropZone(DropZone_New Yard);

        [OperationContract]
        string UpdateDropZone(DropZone_New Yard);

        [OperationContract]
        string DeleteDropZone(Guid DropZoneID);

        [OperationContract]
        List<CallBoxes_New> RetreiveCallBoxes();

        [OperationContract]
        CallBoxes_New RetreiveCallBox(Guid CallBoxID);

        [OperationContract]
        string CreateCallBox(CallBoxes_New Yard);

        [OperationContract]
        string UpdateCallBox(CallBoxes_New Yard);

        [OperationContract]
        string DeleteCallBox(Guid DropZoneID);

        [OperationContract]
        List<Five11Signs> RetreiveFive11Signs();

        [OperationContract]
        Five11Signs RetreiveFive11Sign(Guid Five11SignID);

        [OperationContract]
        string CreateFive11Sign(Five11Signs Five11Sign);

        [OperationContract]
        string UpdateFive11Sign(Five11Signs Five11Sign);

        [OperationContract]
        string DeleteFive11Sign(Guid Five11SignID);
    }

    #endregion

    #region " Classes "

    [DataContract]
    public class ListDrivers
    {
        [DataMember] public Guid TruckID { get; set; }

        [DataMember] public string TruckNumber { get; set; }

        [DataMember] public Guid DriverID { get; set; }

        [DataMember] public string DriverName { get; set; }

        [DataMember] public string ContractorName { get; set; }
    }

    [DataContract]
    public class AssistTruck
    {
        [DataMember] public Guid TruckID { get; set; }

        [DataMember] public string TruckNumber { get; set; }

        [DataMember] public string ContractorName { get; set; }

        [DataMember] public Guid ContractorID { get; set; }
    }

    [DataContract]
    public class AssistReq
    {
        [DataMember] public Guid AssistID { get; set; }

        [DataMember] public Guid IncidentID { get; set; }

        [DataMember] public Guid DriverID { get; set; }

        [DataMember] public Guid FleetVehicleID { get; set; }

        [DataMember] public DateTime DispatchTime { get; set; }

        [DataMember] public int CustomerWaitTime { get; set; }

        [DataMember] public Guid VehiclePositionID { get; set; }

        [DataMember] public Guid IncidentTypeID { get; set; }

        [DataMember] public Guid TrafficSpeedID { get; set; }

        [DataMember] public Guid ServiceTypeID { get; set; }

        [DataMember] public string DropZone { get; set; }

        [DataMember] public string Make { get; set; }

        [DataMember] public string Model { get; set; }

        [DataMember] public Guid VehicleTypeID { get; set; }

        [DataMember] public string Color { get; set; }

        [DataMember] public string LicensePlate { get; set; }

        [DataMember] public string State { get; set; }

        [DataMember] public int StartOD { get; set; }

        [DataMember] public int EndOD { get; set; }

        [DataMember] public Guid TowLocationID { get; set; }

        [DataMember] public string Tip { get; set; }

        [DataMember] public string TipDetail { get; set; }

        [DataMember] public string CustomerLastName { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public bool IsMDC { get; set; }

        [DataMember] public DateTime x1097 { get; set; }

        [DataMember] public DateTime x1098 { get; set; }

        [DataMember] public Guid ContractorID { get; set; }

        [DataMember] public string LogNumber { get; set; }

        [DataMember] public double Lat { get; set; }

        [DataMember] public double Lon { get; set; }
    }


    [DataContract]
    public class TowTruckData
    {
        [DataMember] public string TruckNumber { get; set; }

        [DataMember] public string Direction { get; set; }

        [DataMember] public double Speed { get; set; }

        [DataMember] public double Lat { get; set; }

        [DataMember] public double Lon { get; set; }

        [DataMember] public string VehicleState { get; set; }

        [DataMember] public bool Alarms { get; set; }

        [DataMember] public bool SpeedingAlarm { get; set; }

        [DataMember] public double SpeedingValue { get; set; }

        [DataMember] public DateTime SpeedingTime { get; set; }

        [DataMember] public bool OutOfBoundsAlarm { get; set; }

        [DataMember] public string OutOfBoundsMessage { get; set; }

        [DataMember] public DateTime OutOfBoundsTime { get; set; }

        [DataMember] public double Heading { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public string IPAddress { get; set; }

        [DataMember] public DateTime LastMessage { get; set; }

        [DataMember] public string ContractorName { get; set; }

        [DataMember] public string DriverName { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public DateTime StatusStarted { get; set; }
    }

    [DataContract]
    public class IncidentIn
    {
        [DataMember] public Guid IncidentID { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public int FreewayID { get; set; }

        [DataMember] public Guid LocationID { get; set; }

        [DataMember] public string Direction { get; set; }

        [DataMember] public Guid BeatSegmentID { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public DateTime TimeStamp { get; set; }

        [DataMember] public Guid CreatedBy { get; set; }

        [DataMember] public string Description { get; set; }

        [DataMember] public string IncidentNumber { get; set; }

        [DataMember] public string CrossStreet1 { get; set; }

        [DataMember] public string CrossStreet2 { get; set; }
    }

    [DataContract]
    public class IncidentScreenData
    {
        [DataMember] public Guid IncidentID { get; set; }

        [DataMember] public string Direction { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public string Freeway { get; set; }

        [DataMember] public string TimeStamp { get; set; }

        [DataMember] public string CreatedBy { get; set; }

        [DataMember] public string Description { get; set; }

        [DataMember] public string IncidentNumber { get; set; }

        [DataMember] public string CrossStreet1 { get; set; }

        [DataMember] public string CrossStreet2 { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public string TruckNumber { get; set; }

        [DataMember] public string Driver { get; set; }

        [DataMember] public string State { get; set; }

        [DataMember] public string ContractorName { get; set; }
    }

    [DataContract]
    public class AssistScreenData
    {
        [DataMember] public Guid AssistID { get; set; }

        [DataMember] public string DriverName { get; set; }

        [DataMember] public string DispatchNumber { get; set; }

        [DataMember] public string AssistNumber { get; set; }

        [DataMember] public string IncidentNumber { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public string VehicleNumber { get; set; }

        [DataMember] public string ContractorName { get; set; }

        [DataMember] public DateTime x1097 { get; set; }

        [DataMember] public DateTime OnSiteTime { get; set; }

        [DataMember] public DateTime x0198 { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public double Latitude { get; set; }

        [DataMember] public double Longitude { get; set; }

        [DataMember] public int CustomerWaitTime { get; set; }

        [DataMember] public string VehiclePosition { get; set; }

        [DataMember] public string TrafficSpeed { get; set; }

        [DataMember] public string DropZone { get; set; }

        [DataMember] public string Make { get; set; }

        [DataMember] public string VehicleType { get; set; }

        [DataMember] public string Color { get; set; }

        [DataMember] public string LicensePlate { get; set; }

        [DataMember] public string State { get; set; }

        [DataMember] public double StartOD { get; set; }

        [DataMember] public double EndOD { get; set; }

        [DataMember] public string TowLocation { get; set; }

        [DataMember] public string Tip { get; set; }

        [DataMember] public string TipDetail { get; set; }

        [DataMember] public string CustomerLastName { get; set; }

        [DataMember] public string[] SelectedServices { get; set; }

        [DataMember] public bool AssistAcked { get; set; }

        [DataMember] public bool AssistComplete { get; set; }
    }

    [DataContract]
    public class AlarmStatus
    {
        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public string VehicleNumber { get; set; }

        [DataMember] public string DriverName { get; set; }

        [DataMember] public string ContractCompanyName { get; set; }

        [DataMember] public bool SpeedingAlarm { get; set; }

        [DataMember] public double SpeedingValue { get; set; }

        [DataMember] public DateTime SpeedingTime { get; set; }

        [DataMember] public bool OutOfBoundsAlarm { get; set; }

        [DataMember] public string OutOfBoundsMessage { get; set; }

        [DataMember] public DateTime OutOfBoundsTime { get; set; }

        [DataMember] public DateTime OutOfBoundsStartTime { get; set; }

        [DataMember] public DateTime OutOfBoundsExcused { get; set; }

        [DataMember] public DateTime OutOfBoundsCleared { get; set; }

        [DataMember] public string VehicleStatus { get; set; }

        [DataMember] public DateTime StatusStarted { get; set; }

        [DataMember] public bool LogOnAlarm { get; set; }

        [DataMember] public DateTime LogOnAlarmTime { get; set; }

        [DataMember] public DateTime LogOnAlarmCleared { get; set; }

        [DataMember] public DateTime LogOnAlarmExcused { get; set; }

        [DataMember] public string LogOnAlarmComments { get; set; }

        [DataMember] public bool RollOutAlarm { get; set; }

        [DataMember] public DateTime RollOutAlarmTime { get; set; }

        [DataMember] public DateTime RollOutAlarmCleared { get; set; }

        [DataMember] public DateTime RollOutAlarmExcused { get; set; }

        [DataMember] public string RollOutAlarmComments { get; set; }

        [DataMember] public bool OnPatrolAlarm { get; set; }

        [DataMember] public DateTime OnPatrolAlarmTime { get; set; }

        [DataMember] public DateTime OnPatrolAlarmCleared { get; set; }

        [DataMember] public DateTime OnPatrolAlarmExcused { get; set; }

        [DataMember] public string OnPatrolAlarmComments { get; set; }

        [DataMember] public bool RollInAlarm { get; set; }

        [DataMember] public DateTime RollInAlarmTime { get; set; }

        [DataMember] public DateTime RollInAlarmCleared { get; set; }

        [DataMember] public DateTime RollInAlarmExcused { get; set; }

        [DataMember] public string RollInAlarmComments { get; set; }

        [DataMember] public bool LogOffAlarm { get; set; }

        [DataMember] public DateTime LogOffAlarmTime { get; set; }

        [DataMember] public DateTime LogOffAlarmCleared { get; set; }

        [DataMember] public DateTime LogOffAlarmExcused { get; set; }

        [DataMember] public string LogOffAlarmComments { get; set; }

        [DataMember] public bool IncidentAlarm { get; set; }

        [DataMember] public DateTime IncidentAlarmTime { get; set; }

        [DataMember] public DateTime IncidentAlarmCleared { get; set; }

        [DataMember] public DateTime IncidentAlarmExcused { get; set; }

        [DataMember] public string IncidentAlarmComments { get; set; }

        [DataMember] public bool GPSIssueAlarm { get; set; }

        [DataMember] public DateTime GPSIssueAlarmStart { get; set; }

        [DataMember] public DateTime GPSIssueAlarmCleared { get; set; }

        [DataMember] public DateTime GPSIssueAlarmExcused { get; set; }

        [DataMember] public string GPSIssueAlarmComments { get; set; }

        [DataMember] public bool StationaryAlarm { get; set; }

        [DataMember] public DateTime StationaryAlarmStart { get; set; }

        [DataMember] public DateTime StationaryAlarmCleared { get; set; }

        [DataMember] public DateTime StationaryAlarmExcused { get; set; }

        [DataMember] public string StationaryAlarmComments { get; set; }
    }

    [DataContract]
    public class TruckMessage
    {
        [DataMember] public Guid MessageID { get; set; }

        [DataMember] public string TruckIP { get; set; }

        [DataMember] public string MessageText { get; set; }

        [DataMember] public DateTime SentTime { get; set; }

        [DataMember] public Guid UserID { get; set; }

        [DataMember] public bool Acked { get; set; }

        [DataMember] public DateTime AckedTime { get; set; }
    }

    [DataContract]
    public class IncidentDisplay
    {
        [DataMember] public Incident Incident { get; set; }
        [DataMember] public Assist Assist { get; set; }
        [DataMember] public string TruckNumber { get; set; }
        [DataMember] public string DriverName { get; set; }
        [DataMember] public string State { get; set; }
        [DataMember] public string ContractorName { get; set; }
        [DataMember] public string ContractCompanyName { get; set; }
        [DataMember] public string VehicleTypeName { get; set; }
        [DataMember] public string IncidentTypeName { get; set; }
    }

    #endregion

    #region  " Other Server Classes "

    [DataContract]
    public class CopyTruck
    {
        [DataMember] public string Identifier { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public Guid BeatID { get; set; }

        [DataMember] public CopyStatus Status { get; set; }

        [DataMember] public CopyDriver Driver { get; set; }

        [DataMember] public CopyExtended Extended { get; set; }
    }

    [DataContract]
    public class CopyStatus
    {
        [DataMember] public bool SpeedingAlarm { get; set; }

        [DataMember] public double SpeedingValue { get; set; }

        [DataMember] public DateTime SpeedingTime { get; set; }

        //[DataMember]
        //public SqlGeography SpeedingLocation { get; set; }
        [DataMember] public bool OutOfBoundsAlarm { get; set; }

        [DataMember] public string OutOfBoundsMessage { get; set; }

        [DataMember] public DateTime OutOfBoundsTime { get; set; }

        [DataMember] public DateTime OutOfBoundsStartTime { get; set; }

        [DataMember] public string VehicleStatus { get; set; }

        [DataMember] public DateTime StatusStarted { get; set; }

        [DataMember] public bool LogOnAlarm { get; set; }

        [DataMember] public DateTime LogOnAlarmTime { get; set; }

        [DataMember] public DateTime LogOnAlarmCleared { get; set; }

        [DataMember] public DateTime LogOnAlarmExcused { get; set; }

        [DataMember] public string LogOnAlarmComments { get; set; }

        [DataMember] public bool RollOutAlarm { get; set; }

        [DataMember] public DateTime RollOutAlarmTime { get; set; }

        [DataMember] public DateTime RollOutAlarmCleared { get; set; }

        [DataMember] public DateTime RollOutAlarmExcused { get; set; }

        [DataMember] public string RollOutAlarmComments { get; set; }

        [DataMember] public bool OnPatrolAlarm { get; set; }

        [DataMember] public DateTime OnPatrolAlarmTime { get; set; }

        [DataMember] public DateTime OnPatrolAlarmCleared { get; set; }

        [DataMember] public DateTime OnPatrolAlarmExcused { get; set; }

        [DataMember] public string OnPatrolAlarmComments { get; set; }

        [DataMember] public bool RollInAlarm { get; set; }

        [DataMember] public DateTime RollInAlarmTime { get; set; }

        [DataMember] public DateTime RollInAlarmCleared { get; set; }

        [DataMember] public DateTime RollInAlarmExcused { get; set; }

        [DataMember] public string RollInAlarmComments { get; set; }

        [DataMember] public bool LogOffAlarm { get; set; }

        [DataMember] public DateTime LogOffAlarmTime { get; set; }

        [DataMember] public DateTime LogOffAlarmCleared { get; set; }

        [DataMember] public DateTime LogOffAlarmExcused { get; set; }

        [DataMember] public string LogOffAlarmComments { get; set; }

        [DataMember] public bool IncidentAlarm { get; set; }

        [DataMember] public DateTime IncidentAlarmTime { get; set; }

        [DataMember] public DateTime IncidentAlarmCleared { get; set; }

        [DataMember] public DateTime IncidentAlarmExcused { get; set; }

        [DataMember] public string IncidentAlarmComments { get; set; }

        [DataMember] public bool GPSIssueAlarm { get; set; } //handles NO GPS

        [DataMember] public DateTime GPSIssueAlarmStart { get; set; } //handles NO GPS

        [DataMember] public DateTime GPSIssueAlarmCleared { get; set; }

        [DataMember] public DateTime GPSIssueAlarmExcused { get; set; }

        [DataMember] public string GPSIssueAlarmComments { get; set; }

        [DataMember] public bool StationaryAlarm { get; set; } //handles no movement, speed = 0

        [DataMember] public DateTime StationaryAlarmStart { get; set; } //handles no movement, speed = 0

        [DataMember] public DateTime StationaryAlarmCleared { get; set; }

        [DataMember] public DateTime StationaryAlarmExcused { get; set; }

        [DataMember] public string StationaryAlarmComments { get; set; }
    }

    [DataContract]
    public class CopyDriver
    {
        [DataMember] public Guid DriverID { get; set; }

        [DataMember] public string LastName { get; set; }

        [DataMember] public string FirstName { get; set; }

        [DataMember] public string TowTruckCompany { get; set; }

        [DataMember] public string FSPID { get; set; }

        [DataMember] public Guid AssignedBeat { get; set; }

        [DataMember] public Guid BeatScheduleID { get; set; }

        [DataMember] public DateTime BreakStarted { get; set; }

        [DataMember] public DateTime LunchStarted { get; set; }
    }

    [DataContract]
    public class CopyExtended
    {
        [DataMember] public string ContractorName { get; set; }

        [DataMember] public Guid ContractorID { get; set; }

        [DataMember] public string TruckNumber { get; set; }

        [DataMember] public string FleetNumber { get; set; }

        [DataMember] public DateTime ProgramStartDate { get; set; }

        [DataMember] public string VehicleType { get; set; }

        [DataMember] public int VehicleYear { get; set; }

        [DataMember] public string VehicleMake { get; set; }

        [DataMember] public string VehicleModel { get; set; }

        [DataMember] public string LicensePlate { get; set; }

        [DataMember] public DateTime RegistrationExpireDate { get; set; }

        [DataMember] public DateTime InsuranceExpireDate { get; set; }

        [DataMember] public DateTime LastCHPInspection { get; set; }

        [DataMember] public DateTime ProgramEndDate { get; set; }

        [DataMember] public int FAW { get; set; }

        [DataMember] public int RAW { get; set; }

        [DataMember] public int RAWR { get; set; }

        [DataMember] public int GVW { get; set; }

        [DataMember] public int GVWR { get; set; }

        [DataMember] public int Wheelbase { get; set; }

        [DataMember] public int Overhang { get; set; }

        [DataMember] public int MAXTW { get; set; }

        [DataMember] public DateTime MAXTWCALCDATE { get; set; }

        [DataMember] public string FuelType { get; set; }

        [DataMember] public Guid FleetVehicleID { get; set; }
    }

    #endregion

    #region "Geo Object Data Models"

    [DataContract]
    public class BeatSegment_New
    {
        [DataMember] public Guid BeatSegmentID { get; set; }

        [DataMember] public string CHPDescription { get; set; }

        [DataMember] public string PIMSID { get; set; }

        [DataMember] public string BeatSegmentExtent { get; set; }

        [DataMember] public string BeatSegmentNumber { get; set; }

        [DataMember] public string BeatSegmentDescription { get; set; }

        [DataMember] public string CHPDescription2 { get; set; }

        [DataMember] public string LastUpdate { get; set; }

        [DataMember] public string LastUpdateBy { get; set; }

        [DataMember] public bool Active { get; set; }

        [DataMember] public string Color { get; set; }
    }

    [DataContract]
    public class BeatSegment_Cond
    {
        [DataMember] public Guid BeatSegmentID { get; set; }

        [DataMember] public string BeatSegmentNumber { get; set; }

        [DataMember] public string BeatSegmentDescription { get; set; }

        [DataMember] public string Color { get; set; }

        [DataMember] public string BeatSegmentExtent { get; set; }
    }

    [DataContract]
    public class Beats_New
    {
        [DataMember] public Guid BeatID { get; set; }

        [DataMember] public bool Active { get; set; }

        [DataMember] public string BeatExtent { get; set; }

        [DataMember] public int FreewayID { get; set; }

        [DataMember] public string BeatDescription { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public DateTime LastUpdate { get; set; }

        [DataMember] public string LastUpdateBy { get; set; }

        [DataMember] public bool IsTemporary { get; set; }

        [DataMember] public string BeatColor { get; set; }

        [DataMember] public DateTime StartDate { get; set; }

        [DataMember] public DateTime EndDate { get; set; }

        [DataMember] public List<BeatSegment_New> BeatSegments { get; set; }
    }

    [DataContract]
    public class Beats_New2
    {
        [DataMember] public Guid BeatID { get; set; }

        [DataMember] public bool Active { get; set; }

        [DataMember] public string BeatExtent { get; set; }

        [DataMember] public int FreewayID { get; set; }

        [DataMember] public string BeatDescription { get; set; }

        [DataMember] public string BeatNumber { get; set; }

        [DataMember] public DateTime LastUpdate { get; set; }

        [DataMember] public string LastUpdateBy { get; set; }

        [DataMember] public bool IsTemporary { get; set; }

        [DataMember] public string BeatColor { get; set; }

        [DataMember] public DateTime StartDate { get; set; }

        [DataMember] public DateTime EndDate { get; set; }

        [DataMember] public List<Guid> BeatSegments { get; set; }
    }


    [DataContract]
    public class latLng
    {
        [DataMember] public string lat { get; set; }

        [DataMember] public string lng { get; set; }
    }

    [DataContract]
    public class Yard_New
    {
        [DataMember] public Guid YardID { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public string TowTruckCompanyName { get; set; }

        [DataMember] public string Position { get; set; }

        [DataMember] public string YardDescription { get; set; }

        [DataMember] public string TowTruckCompanyPhoneNumber { get; set; }
    }

    [DataContract]
    public class DropZone_New
    {
        [DataMember] public Guid DropZoneID { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public string Restrictions { get; set; }

        [DataMember] public string DropZoneNumber { get; set; }

        [DataMember] public string DropZoneDescription { get; set; }

        [DataMember] public string City { get; set; }

        [DataMember] public string PDPhoneNumber { get; set; }

        [DataMember] public int Capacity { get; set; }

        [DataMember] public string Position { get; set; }
    }

    [DataContract]
    public class CallBoxes_New
    {
        [DataMember] public Guid CallBoxID { get; set; }

        [DataMember] public string TelephoneNumber { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public int FreewayID { get; set; }

        [DataMember] public string SiteType { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public string Position { get; set; }

        [DataMember] public string SignNumber { get; set; }
    }

    [DataContract]
    public class Five11Signs
    {
        [DataMember] public Guid Five11SignID { get; set; }

        [DataMember] public string TelephoneNumber { get; set; }

        [DataMember] public string Location { get; set; }

        [DataMember] public int FreewayID { get; set; }

        [DataMember] public string SiteType { get; set; }

        [DataMember] public string Comments { get; set; }

        [DataMember] public string Position { get; set; }

        [DataMember] public string SignNumber { get; set; }
    }

    #endregion
}