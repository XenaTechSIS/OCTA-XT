using System;

namespace FSP.Web.Models
{
    public class UIUser
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
        public Guid? ContractorID { get; set; }
    }
}