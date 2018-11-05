using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.TowTruckServiceRef;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class IncidentController : Controller
    {
        public ActionResult GetIncidents()
        {
            var contractorName = string.Empty;

            using (var dc = new FSPDataContext())
            {
                var user = dc.Users.FirstOrDefault(p => p.Email == User.Identity.Name);
                if (user != null && user.Role.RoleName == "Contractor")
                {
                    contractorName = dc.Contractors.FirstOrDefault(p => p.ContractorID == user.ContractorID)?.ContractCompanyName;
                }
            }

            using (var service = new TowTruckServiceClient())
            {
                 var rawIncidents = service.getIncidentData();

                var incidents = from q in rawIncidents
                                select new
                                {
                                    q.IncidentID,
                                    q.IsIncidentComplete,
                                    q.IsAcked,
                                    //UI fields
                                    q.IncidentNumber,
                                    q.BeatNumber,
                                    q.TruckNumber,
                                    q.DriverName,
                                    q.DispatchComments,
                                    Timestamp = q.Timestamp.ToString(),
                                    q.State,
                                    q.contractor.ContractCompanyName,
                                    q.Assists
                                };

                if (!string.IsNullOrEmpty(contractorName))
                {
                    incidents = incidents.Where(p => p.ContractCompanyName == contractorName);
                }
                return Json(incidents.OrderBy(p => p.BeatNumber), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}