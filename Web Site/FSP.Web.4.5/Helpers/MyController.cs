using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;

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

        [OutputCache(Duration = 5)]
        public ActionResult GetBeatNumbers()
        {
            using (var db = new FSPDataContext())
            {
                if (!string.IsNullOrEmpty(UsersContractorCompanyName))
                {
                    //HOW DO I FITER BY CONTACTOR?
                    var data = db.vBeats.OrderBy(p => p.BeatNumber).Select(q => new
                    {
                        Id = q.BeatNumber,
                        Name = q.BeatNumber
                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = db.vBeats.OrderBy(p => p.BeatNumber).Select(q => new
                    {
                        Id = q.BeatNumber,
                        Name = q.BeatNumber
                    }).ToList();
                    return Json(data.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult CurrentUserIsAdmin()
        {
            var isAdmin = HttpContext.User.IsInRole("Admin");
            return Json(isAdmin, JsonRequestBehavior.AllowGet);
        }
    }
}