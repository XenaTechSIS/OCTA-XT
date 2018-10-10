using System;

namespace FSP.Web.Models
{
    public class UICallBox
    {
        public Guid CallBoxId { get; set; }
        public string TelephoneNumber { get; set; }
        public int FreewayId { get; set; }
        public string LocationDescription { get; set; }
        public string SiteType { get; set; }
        public string Comments { get; set; }
        public string SignNumber { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}