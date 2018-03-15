using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSP.Web.Models
{
    public class AlarmHistory
    {
        public String BeatNumber { get; set; }
        public String ContractCompanyName { get; set; }
        public String VehicleNumber { get; set; }
        public String DriverName { get; set; }
        public DateTime AlarmTime { get; set; }
        public String AlarmType { get; set; }
        public String Comments { get; set; }
        public String ExcuseTime { get; set; }

        public bool IsExcused { get; set; }
    }
}