using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FSP.Web.Models
{
    public class UIIncident
    {
        //Incident Type
        [Required]
        [DisplayName("Incident Type")]
        public string SelectedIncidentType { get; set; }

        //Freeway
        [Required] [DisplayName("Freeway")] public int SelectedFreeway { get; set; }


        //Beats (depending on freeway selection)
        [Required] [DisplayName("Beat")] public string SelectedBeat { get; set; }

        //Segment (depending on freeway selection)
        [Required] [DisplayName("Segment")] public string SelectedSegment { get; set; }

        [Required]
        [DisplayName("Vehicle Position")]
        public string SelectedVehicilePosition { get; set; }

        //Vehicle Description
        [StringLength(400)]
        [DisplayName("Vehicle Description")]
        public string VehicleDescription { get; set; }

        //Dispatch Comments
        [StringLength(400)]
        [DisplayName("Dispatch Comments")]
        public string DispatchComments { get; set; }

        public Guid IncidentID { get; set; }
        public string IncidentNumber { get; set; }
        public string BeatNumber { get; set; }
        public string TruckNumber { get; set; }
        public string DriverName { get; set; }
        public string Timestamp { get; set; }
        public string State { get; set; }
        public string DispatchNumber { get; set; }
        public string ContractorName { get; set; }
        public string AssistNumber { get; set; }

        /// <summary>
        ///     An Incident is active if any of the assists is still active.
        /// </summary>
        public bool IsIncidentComplete { get; set; }

        public bool IsAcked { get; set; }
    }
}