//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportServer.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class BeatSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BeatSchedule()
        {
            this.BeatBeatSchedules = new HashSet<BeatBeatSchedule>();
        }
    
        public System.Guid BeatScheduleID { get; set; }
        public string ScheduleName { get; set; }
        public bool Weekday { get; set; }
        public System.TimeSpan Logon { get; set; }
        public System.TimeSpan RollOut { get; set; }
        public System.TimeSpan OnPatrol { get; set; }
        public System.TimeSpan RollIn { get; set; }
        public System.TimeSpan LogOff { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int BreakDuration { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BeatBeatSchedule> BeatBeatSchedules { get; set; }
    }
}
