using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FSP.Domain.Model
{
    [MetadataType(typeof(DriverInteractionMetaData))]
    public partial class DriverInteraction
    {
    }

    public class DriverInteractionMetaData
    {
        [Required]
        [DisplayName("Contractor")]
        public int ContractorID { get; set; }

        [Required]
        [DisplayName("Driver")]
        public Guid DriverID { get; set; }

        [Required]
        [DisplayName("Interaction Type")]
        public Guid InteractionTypeID { get; set; }

        [Required]
        [DisplayName("Interaction Description")]
        [StringLength(100)]
        public String InteractionArea { get; set; }

        [Required]
        [DisplayName("Interaction Narrative")]
        [StringLength(500)]
        public String InteractionDescription { get; set; }

        [Required]
        [DisplayName("Inspection Pass Fail")]       
        public Boolean InspectionPassFail { get; set; }

        [Required]
        [DisplayName("Accident Preventable")]
        public Boolean AccidentPreventable { get; set; }

        [Required]
        [DisplayName("Follow Up Required")]
        public Boolean FollowupRequired { get; set; }
       
        [DisplayName("Follow Up Narrative")]
        [StringLength(500)]
        public String FollowupDescription { get; set; }
      
        [DisplayName("Follow Up Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FollowupDate { get; set; }

        [DisplayName("Follow Up Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FollowupCompletionDate { get; set; }

        [DisplayName("Follow Up Comments")]
        [StringLength(500)]
        public String FollowupComments { get; set; }

        [DisplayName("Close Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CloseDate { get; set; }

        [DisplayName("Badge ID")]
        [StringLength(50)]
        public String BadgeID { get; set; }

        [DisplayName("Interaction Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? InteractionDate { get; set; }

        [DisplayName("Vehicle Number")]
        [StringLength(50)]
        public String VehicleNumber { get; set; }

        [DisplayName("Beat Number")]
        [StringLength(50)]
        public String BeatNumber { get; set; }

    }
}
