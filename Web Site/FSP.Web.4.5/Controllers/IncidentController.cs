using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class IncidentController : Controller
    {
        public ActionResult GetIncidents()
        {
            var returnList = new List<UIIncident>();
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
                try
                {
                    var allIncidents = from q in service.getIncidentData()
                                       select new UIIncident
                                       {
                                           IncidentID = q.IncidentID,
                                           IsIncidentComplete = q.IsIncidentComplete,
                                           IsAcked = q.IsAcked,
                                           //UI fields
                                           IncidentNumber = q.IncidentNumber,
                                           BeatNumber = q.BeatNumber,
                                           TruckNumber = q.TruckNumber,
                                           DriverName = q.DriverName,
                                           DispatchComments = q.DispatchComments,
                                           Timestamp = q.Timestamp.ToString(),
                                           State = q.State,
                                           DispatchNumber = q.IncidentNumber,
                                           ContractorName = q.ContractorName,
                                           AssistNumber = q.AssistNumber
                                       };


                    foreach (var incident in allIncidents)
                    {
                        var addTruck = true;
                        if (!string.IsNullOrEmpty(contractorName))
                        {
                            addTruck = contractorName == incident.ContractorName;
                        }                                                    
                        if (addTruck)
                            returnList.Add(incident);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return Json(returnList.OrderBy(p => p.BeatNumber), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}