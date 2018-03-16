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
            var addTruck = true;

            using (var dc = new FSPDataContext())
            {
                var user = dc.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();

                if (user.Role.RoleName == "Contractor")
                    contractorName = dc.Contractors.Where(p => p.ContractorID == user.ContractorID).FirstOrDefault()
                        .ContractCompanyName;
                else
                    contractorName = string.Empty;
            }

            using (var service = new TowTruckServiceClient())
            {
                try
                {
                    var allIncidents = from q in service.getIncidentData()
                        select new UIIncident
                        {
                            IncidentID = q.IncidentID,
                            IncidentNumber = q.IncidentNumber,
                            BeatNumber = q.BeatNumber,
                            TruckNumber = q.TruckNumber,
                            DriverName = q.DriverName,
                            DispatchComments = q.DispatchComments,
                            Timestamp = q.Timestamp.ToString(),
                            State = q.State,
                            DispatchNumber = q.IncidentNumber,
                            ContractorName = q.ContractorName,
                            IsIncidentComplete = q.IsIncidentComplete,
                            IsAcked = q.IsAcked
                        };


                    foreach (var incident in allIncidents)
                    {
                        if (!string.IsNullOrEmpty(contractorName))
                            if (contractorName == incident.ContractorName)
                                addTruck = true;
                            else
                                addTruck = false;
                        else
                            addTruck = true;

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