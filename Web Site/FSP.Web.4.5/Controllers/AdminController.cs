using System;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Models;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class AdminController : Controller
    {
        private readonly FSPDataContext db = new FSPDataContext();

        //[CustomAuthorization(Roles = "Admin, Dispatcher")]
        //public ActionResult CreateIncident()
        //{
        //    var model = CreateUIIncidentModel();
        //    return View(model);
        //}

        //[HttpPost]
        //[CustomAuthorization(Roles = "Admin, Dispatcher")]
        //public ActionResult CreateIncident(UIIncident incident)
        //{
        //    if (ModelState.IsValid) return RedirectToAction("IncidentCreated");

        //    var model = CreateUIIncidentModel();
        //    return View();
        //}

        //[CustomAuthorization(Roles = "Admin, Dispatcher")]
        //public ActionResult Dispatch()
        //{
        //    return View();
        //}

        //[CustomAuthorization(Roles = "Admin, Dispatcher")]
        //public ActionResult IncidentCreated()
        //{
        //    return View();
        //}

        //[CustomAuthorization(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db?.Dispose();
            base.Dispose(disposing);
        }

        //#region helpers

        //private UIIncident CreateUIIncidentModel()
        //{
        //    //MVC likes ViewBags better for reposts. I tried to have the list part of the model but no good.
        //    ViewBag.IncidentTypes = db.IncidentTypes.OrderBy(p => p.IncidentType1).ToList();
        //    ViewBag.Freeways = db.Freeways.OrderBy(p => p.FreewayName).ToList();
        //    ViewBag.VehiclePositions = db.VehiclePositions.OrderBy(p => p.VehiclePosition1).ToList();

        //    var model = new UIIncident();

        //    return model;
        //}

        //public ActionResult GetBeatsByFreewayId(int freewayId)
        //{
        //    var beats = from q in db.vBeats
        //        where q.FreewayID == freewayId
        //        orderby q.BeatNumber
        //        select new
        //        {
        //            q.BeatID,
        //            q.BeatNumber
        //        };

        //    return Json(beats, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetSegmentsByBeatId(Guid beatId)
        //{
        //    var beats = from q in db.BeatBeatSegments
        //        join s in db.vBeatSegments on q.BeatSegmentID equals s.BeatSegmentID
        //        orderby s.BeatSegmentNumber
        //        where q.BeatID == beatId
        //        select new
        //        {
        //            s.BeatSegmentID,
        //            s.BeatSegmentNumber
        //        };

        //    return Json(beats, JsonRequestBehavior.AllowGet);
        //}

        //#endregion
    }
}