using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.TowTruckServiceRef;
using System.Linq;
using System.Web.Mvc;

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

                using (var service = new TowTruckServiceClient())
                {
                    var rawIncidents = service.GetIncidentData().ToList();

                    var incidents = from q in rawIncidents
                                    select new
                                    {
                                        q.Incident,
                                        q.Assist,

                                        q.TruckNumber,
                                        q.DriverName,

                                        q.State,
                                        q.ContractorName,
                                        q.ContractCompanyName,

                                        q.VehicleTypeName,
                                        q.IncidentTypeName,
                                        q.SelectedService
                                    };


                    if (!string.IsNullOrEmpty(contractorName))
                    {
                        incidents = incidents.Where(p => p.ContractorName == contractorName);
                    }
                    return Json(incidents.OrderBy(p => p.Incident.BeatNumber), JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}