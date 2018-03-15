﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSP.Domain.Model
{
    public class LocalBeatSchedule
    {
        public string BeatNumber { get; set; }
        public string ScheduleName { get; set; }
        public string ScheduleTimeTable { get; set; }
        public string Supervisor { get; set; }
        public string PhoneNumber { get; set; }
        public string CellPhone { get; set; }
        public string ContractCompanyName { get; set; }
        public string Weekday { get; set; }
    }
}
