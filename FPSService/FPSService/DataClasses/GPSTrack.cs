using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPSService.DataClasses
{
    public class GPSTrack
    {
        public int ID { get; set; }
        public double Direction { get; set; }
        public string VehicleStatus { get; set; }
        public string timeStamp { get; set; }
        public string VehicleID { get; set; }
        public double Speed { get; set; }
        public string DriverName { get; set; }
        public string ContractCompanyName { get; set; }
        public string IPAddress { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double SpeedingValue { get; set; }
        public DateTime SpeedingTime { get; set; }
        public string OutOfBoundsMessage { get; set; }
        public string BeatNumber { get; set; }
    }
}