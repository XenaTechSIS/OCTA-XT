using System.Collections.Generic;

namespace FSP.Web.Models
{
    public class UIMapPolygonLine
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public List<UIMapPolygonLinePoint> Points { get; set; }
        public string Color { get; set; }
    }
}