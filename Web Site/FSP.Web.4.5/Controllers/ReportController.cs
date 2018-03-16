using System.Web.Mvc;
using FSP.Web.Filters;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class ReportController : Controller
    {       
        public ActionResult Index()
        {
            return View();
        }
    }
}