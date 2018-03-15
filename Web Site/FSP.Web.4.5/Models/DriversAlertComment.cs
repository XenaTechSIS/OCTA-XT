using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSP.Web.Models
{
    public class DriversAlertComment
    {
        public String BeatNumber { get; set; }
        public String VehicleNumber { get; set; }
        public String DriverFirstName { get; set; }
        public String DriverLastName { get; set; }
        public String DriverFullName { get; set; }
        public String Datestamp { get; set; }
        public String Explanation { get; set; }
        public String CHPLogNumber { get; set; }
        public String ExceptionType { get; set; }
    }
}