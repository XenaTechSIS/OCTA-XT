using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPSService.MiscData
{
    public class IncidentType
    {
        public Guid IncidentTypeID { get; set; }
        public string IncidentTypeCode { get; set; }
        public string IncidentTypeName { get; set; }
    }
}