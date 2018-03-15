using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class AssistController : Controller
    {      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAssists()
        {
            var returnList = new List<UIAssist>();
            var contractorName = string.Empty;

            using (var dc = new FSPDataContext())
            {
                var user = dc.Users.FirstOrDefault(p => p.Email == User.Identity.Name);
                if (user != null)
                {
                    contractorName = user.Role.RoleName == "Contractor" ? dc.Contractors.FirstOrDefault(p => p.ContractorID == user.ContractorID)?.ContractCompanyName : string.Empty;
                }                
            }

            using (var service = new TowTruckServiceClient())
            {
                var allAssists = from q in service.GetAllAssists()
                                 select new UIAssist
                                 {
                                     AssistNumber = q.AssistNumber,
                                     BeatNumber = q.BeatNumber,
                                     Color = q.Color,
                                     DispatchNumber = q.DispatchNumber,
                                     //OnSiteTime = q.OnSiteTime,
                                     //x1097 = q.x1097,
                                     //x1098 = q.x0198,
                                     Driver = q.DriverName,
                                     DriverComments = q.Comments,
                                     DropZone = q.DropZone,
                                     Make = q.Make,                                     
                                     PlateNumber = q.LicensePlate,
                                     ContractorName = q.ContractorName,
                                     x1097 = q.x1097.ToString("MMM dd, hh:mm tt"),
                                     x1098 = q.x0198.ToString("MMM dd, hh:mm tt")
                                 };

                foreach (var assist in allAssists)
                {
                    var addTruck = true;
                    if (!string.IsNullOrEmpty(contractorName))                    
                        addTruck = contractorName == assist.ContractorName;                                       
                    if (addTruck)
                        returnList.Add(assist);
                }
            }

            return Json(returnList.OrderBy(p => p.BeatNumber), JsonRequestBehavior.AllowGet);
        }

    }
}
