using System.Web.Mvc;
using FSP.Web.Filters;

namespace FSP.Web.Controllers
{
    [CustomAuthorization(Roles = "Admin, CHP, Contractor")]
    public class CHPInformationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}