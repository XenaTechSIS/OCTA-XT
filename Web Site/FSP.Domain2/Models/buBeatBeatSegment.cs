using System;
using System.Collections.Generic;

namespace FSP.Domain2.Models
{
    public partial class buBeatBeatSegment
    {
        public System.Guid BeatBeatSegmentID { get; set; }
        public System.Guid BeatID { get; set; }
        public System.Guid BeatSegmentID { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
