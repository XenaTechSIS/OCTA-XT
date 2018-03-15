using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSP.Web.Helpers
{
    public class MyController : Controller
    {
        public String UsersContractorCompanyName { get; set; }
      
        public MyController()
        {
            if (System.Web.HttpContext.Current.User != null)
            {
                if (System.Web.HttpContext.Current.User.IsInRole("Contractor"))
                {
                    using (FspCommon common = new FspCommon())
                    {

                        var contractorUsers = common.GetContractorUsers();
                        var contractors = common.GetContractors();
                        var contractorUser = contractorUsers.Where(p => p.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

                        if (contractorUser != null && contractors != null)
                        {
                            this.UsersContractorCompanyName = contractors.Where(p => p.ContractorID == contractorUser.ContractorID).FirstOrDefault().ContractCompanyName;
                        }
                        else
                            this.UsersContractorCompanyName = String.Empty;

                    }
                } 
            }

            
        }
    }
}