using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Helpers;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;

namespace FSP.Web.Controllers
{
    [CustomAuthorization(Roles = "Admin, Dispatcher, CHP")]
    public class DispatchController : Controller
    {
        private readonly FSPDataContext dc = new FSPDataContext();

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Junk()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            dc?.Dispose();
            base.Dispose(disposing);
        }

        #region Other Calls

        public ActionResult GetDirections(string name_startsWith)
        {
            var directions = new List<string>();
            directions.Add("N");
            directions.Add("S");
            directions.Add("W");
            directions.Add("E");

            return Json(directions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFreewaysByDirection(string name_startsWith)
        {
            var freeways = from q in dc.Freeways
                where q.FreewayName.StartsWith(name_startsWith)
                orderby q.FreewayName
                select q.FreewayName;

            return Json(freeways, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Make this later role depending
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public ActionResult GetLocations(string name_startsWith)
        {
            var locations = from q in dc.Locations
                where q.Location1.StartsWith(name_startsWith)
                orderby q.Location1
                select q.LocationCode;

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCrossStreetFromBeatSegmentDescription(string name_startsWith)
        {
            var beatsDescription = from q in dc.vBeatSegments
                where q.BeatSegmentDescription.Contains(name_startsWith)
                orderby q.BeatSegmentDescription
                select q.BeatSegmentDescription;

            return Json(beatsDescription, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeatsFromDispatchInputForm(string beatSegmentDescription)
        {
            //var beats = from b in dc.vBeats
            //            join bbs in dc.BeatBeatSegments on b.BeatID equals bbs.BeatID
            //            join bs in dc.vBeatSegments on bbs.BeatSegmentID equals bs.BeatSegmentID
            //            where bs.BeatSegmentDescription.Contains(beatSegmentDescription)
            //            select b.BeatNumber;


            //return Json(beats, JsonRequestBehavior.AllowGet);

            var beatSegments = from bs in dc.vBeatSegments
                where bs.BeatSegmentDescription.Contains(beatSegmentDescription)
                select bs.BeatSegmentNumber;

            return Json(beatSegments, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PostIncidentDispatch(UIIncidentDispatch incidentDispatch)
        {
            Util.CreateAssist(incidentDispatch);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PostIncidentDispatchAjax(string direction, string freeway, string location,
            string crossStreet1, string crossStreet2, string comments, string selectedTrucks)
        {
            var returnValue = true;

            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var thisIncident = new IncidentIn();

                    var IncidentID = Guid.NewGuid();
                    var TimeStamp = DateTime.Now;

                    var CreatedBy = MembershipExtensions.GetUserId();
                    var trucks = new JavaScriptSerializer().Deserialize<IEnumerable<SelectedTruck>>(selectedTrucks);


                    var firstTruckBeatNumber = trucks.FirstOrDefault().beatNumberString;

                    thisIncident.IncidentID = IncidentID;
                    thisIncident.Direction = direction;
                    thisIncident.FreewayID = Convert.ToInt32(freeway);
                    thisIncident.LocationID =
                        dc.Locations.Where(p => p.LocationCode == location).FirstOrDefault().LocationID;
                    thisIncident.BeatSegmentID = new Guid("00000000-0000-0000-0000-000000000000");
                    thisIncident.BeatNumber = firstTruckBeatNumber;
                    thisIncident.TimeStamp = TimeStamp;
                    thisIncident.CreatedBy = CreatedBy;
                    thisIncident.Description = comments;
                    thisIncident.CrossStreet1 = crossStreet1;
                    thisIncident.CrossStreet2 = crossStreet2;
                    thisIncident.IncidentNumber = string.Empty;
                    thisIncident.Location = "na";

                    //create incident
                    service.AddIncident(thisIncident);


                    //for each truck create assit

                    foreach (var truck in trucks)
                    {
                        var thisAssist = new AssistReq();
                        thisAssist.AssistID = Guid.NewGuid();

                        thisAssist.IncidentID = IncidentID;

                        var dbTruck = dc.FleetVehicles.Where(p => p.VehicleNumber == truck.truckNumber)
                            .FirstOrDefault();

                        thisAssist.FleetVehicleID = dbTruck.FleetVehicleID;
                        thisAssist.ContractorID = dbTruck.ContractorID;


                        thisAssist.DispatchTime = DateTime.Now;
                        thisAssist.x1097 = DateTime.Now;
                        service.AddAssist(thisAssist);
                    }
                }
            }
            catch
            {
                returnValue = false;
            }


            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }

    public class SelectedTruck
    {
        public string id { get; set; }
        public string truckNumber { get; set; }
        public string beatNumberString { get; set; }
        public string ipAddress { get; set; }
    }
}