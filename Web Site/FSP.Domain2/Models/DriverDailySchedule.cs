using System;
using System.Collections.Generic;

namespace FSP.Domain2.Models
{
    public partial class DriverDailySchedule
    {
        public System.Guid DriverID { get; set; }
        public System.Guid BeatScheduleID { get; set; }
        public System.Guid BeatID { get; set; }
        public virtual Beat Beat { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
