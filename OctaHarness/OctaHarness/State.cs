﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctaHarness
{
    public class State
    {
        public int Id { get; set; }
        public string CarID { get; set; } // may need to reformat to truckid
        public int GpsRate { get; set; }
        public string Log { get; set; }
        public string Version { get; set; }
        public string ServerIP { get; set; }
        public string SFTPServerIP { get; set; }
    }
}
