using System;

namespace FSP.Web.Models
{
    public class AlarmHistory
    {
        public string BeatNumber { get; set; }
        public string ContractCompanyName { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public DateTime AlarmTime { get; set; }
        public string AlarmType { get; set; }
        public string Comments { get; set; }
        public string ExcuseTime { get; set; }

        public bool IsExcused { get; set; }
    }
}