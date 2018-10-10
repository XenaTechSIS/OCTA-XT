using System.Collections.Generic;
using System.ComponentModel;

namespace FSP.Web.Models
{
    public class UIIncidentDispatch
    {
        [DisplayName("Direction")] public string DirectionName { get; set; }

        [DisplayName("Freeway")] public string FreewayId { get; set; }

        [DisplayName("Location")] public string LocationName { get; set; }

        public List<string> SelectedBeatSegments { get; set; }

        public string Description { get; set; }

        public List<string> SelectedTrucks { get; set; }
    }
}