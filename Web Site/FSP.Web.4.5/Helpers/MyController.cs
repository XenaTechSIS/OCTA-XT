using System.Linq;
using System.Web.Mvc;

namespace FSP.Web.Helpers
{
    public class MyController : Controller
    {
        public MyController()
        {
            if (System.Web.HttpContext.Current.User == null) return;

            if (!System.Web.HttpContext.Current.User.IsInRole("Contractor")) return;

            using (var common = new FspCommon())
            {
                var contractorUsers = common.GetContractorUsers();
                var contractors = common.GetContractors();
                var contractorUser = contractorUsers.FirstOrDefault(p =>
                    p.Email == System.Web.HttpContext.Current.User.Identity.Name);

                if (contractorUser != null && contractors != null)
                    UsersContractorCompanyName = contractors
                        .FirstOrDefault(p => p.ContractorID == contractorUser.ContractorID)
                        ?.ContractCompanyName;
                else
                    UsersContractorCompanyName = string.Empty;
            }
        }

        public string UsersContractorCompanyName { get; set; }
    }
}