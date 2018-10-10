namespace FSP.Web.Models
{
    public class DriversAlertComment
    {
        public string BeatNumber { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverFullName { get; set; }
        public string Datestamp { get; set; }
        public string Explanation { get; set; }
        public string CHPLogNumber { get; set; }
        public string ExceptionType { get; set; }
    }
}