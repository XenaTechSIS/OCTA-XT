using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Helpers;

namespace FSP.Web.Controllers
{
    [CustomAuthorization(Roles = "Admin, Contractor")]
    public class AssistAdminController : MyController
    {
        private readonly FSPDataContext dc = new FSPDataContext();

        public ActionResult Create()
        {
            return View();
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetBeatNumbers()
        {
            if (!string.IsNullOrEmpty(UsersContractorCompanyName))
            {
                //HOW DO I FITER BY CONTACTOR?
                var query = from q in dc.vBeats
                    select new
                    {
                        Id = q.BeatNumber,
                        Name = q.BeatNumber
                    };
                return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = from q in dc.vBeats
                    select new
                    {
                        Id = q.BeatNumber,
                        Name = q.BeatNumber
                    };
                return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
            }
        }

        //[OutputCache(Duration = 5)]
        public ActionResult GetContractors()
        {
            var data = (from q in dc.Contractors
                select new
                {
                    Id = q.ContractorID,
                    Name = q.ContractCompanyName
                }).ToList();

            if (!string.IsNullOrEmpty(UsersContractorCompanyName))
                data = data.Where(p => p.Name == UsersContractorCompanyName).ToList();

            return Json(data.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 5)]
        public ActionResult GetDrivers()
        {
            var data = from q in dc.Drivers
                select new
                {
                    Id = q.DriverID,
                    Name = q.LastName + ", " + q.FirstName,
                    ContractorName = q.Contractor.ContractCompanyName
                };

            if (!string.IsNullOrEmpty(UsersContractorCompanyName))
                data = data.Where(p => p.ContractorName == UsersContractorCompanyName);

            return Json(data.OrderBy(p => p.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetFreeways()
        {
            var query = from q in dc.Freeways
                select new
                {
                    Id = q.FreewayID,
                    Name = q.FreewayName
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetIncidentTypes()
        {
            var query = from q in dc.IncidentTypes
                select new
                {
                    Id = q.IncidentTypeID,
                    Name = q.IncidentTypeCode + " (" + q.IncidentType1 + ")"
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetServiceTypes()
        {
            var query = from q in dc.ServiceTypes
                select new
                {
                    Id = q.ServiceTypeID,
                    Code = q.ServiceTypeCode,
                    Name = q.ServiceType1
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetTowLocations()
        {
            var query = from q in dc.TowLocations
                select new
                {
                    Id = q.TowLocationID,
                    Name = q.TowLocationCode + " (" + q.TowLocation1 + ")"
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetTrafficSpeeds()
        {
            var query = from q in dc.TrafficSpeeds
                select new
                {
                    Id = q.TrafficSpeedID,
                    Name = q.TrafficSpeedCode
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetVehiclePositions()
        {
            var query = from q in dc.VehiclePositions
                select new
                {
                    Id = q.VehiclePositionID,
                    Name = q.VehiclePositionCode
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 5)]
        public ActionResult GetVehicles()
        {
            var data = from q in dc.FleetVehicles
                select new
                {
                    Id = q.FleetVehicleID,
                    Name = q.VehicleNumber,
                    ContractorName = q.Contractor.ContractCompanyName
                };

            if (!string.IsNullOrEmpty(UsersContractorCompanyName))
                data = data.Where(p => p.ContractorName == UsersContractorCompanyName);

            return Json(data.OrderBy(p => p.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetVehicleTypes()
        {
            var query = from q in dc.VehicleTypes
                select new
                {
                    Id = q.VehicleTypeID,
                    Name = q.VehicleTypeCode
                };
            return Json(query.OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
    }
}