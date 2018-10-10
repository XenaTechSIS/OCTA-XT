using System;
using System.Collections.Generic;

namespace FSP.Web.Models
{
    public class UIDropZone
    {
        public Guid DropZoneID { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public string DropZoneNumber { get; set; }
        public string DropZoneDescription { get; set; }
        public List<UIMapPolygonLinePoint> PolygonPoints { get; set; }
    }
}