using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPSService.MiscData
{
    public class BeatSchedule
    {
        public Guid BeatID { get; set; }
        public Guid BeatScheduleID { get; set; }
        public string BeatNumber { get; set; }
        public string ScheduleName { get; set; }
        public bool Weekday { get; set; }
        public DateTime Logon { get; set; }
        public DateTime RollOut { get; set; }
        public DateTime OnPatrol { get; set; }
        public DateTime RollIn { get; set; }
        public DateTime LogOff { get; set; }
    }
}