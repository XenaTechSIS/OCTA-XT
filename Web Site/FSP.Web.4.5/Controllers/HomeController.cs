using System.Web.Mvc;
using FSP.Web.Helpers;

namespace FSP.Web.Controllers
{
    public class HomeController : MyController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return View();
        }
    }
}