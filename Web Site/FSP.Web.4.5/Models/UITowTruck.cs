namespace FSP.Web.Models
{
    public class UITowTruck
    {
        public string TruckNumber { get; set; }
        public string VehicleStateIconUrl { get; set; }
        public string VehicleState { get; set; }
        public double Heading { get; set; }
        public string BeatNumber { get; set; }
        public string BeatSegmentNumber { get; set; }
        public int ContractorId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Speed { get; set; }
        public int LastUpdate { get; set; }
        public string LastMessage { get; set; }
        public string LastStatusChanged { get; set; }
        public string DriverName { get; set; }
        public string ContractorName { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }

        public string SpeedingTime { get; set; }
        public string SpeedingValue { get; set; }
        public string OutOfBoundsMessage { get; set; }
        public string OutOfBoundsTime { get; set; }
        public bool HasAlarm { get; set; }
        public string UserContractorName { get; set; }

        //internal prop
        public bool Old { get; set; }
    }
}