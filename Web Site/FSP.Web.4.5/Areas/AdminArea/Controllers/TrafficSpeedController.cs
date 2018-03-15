using System;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;

namespace FSP.Web.Areas.AdminArea.Controllers
{
    [CustomAuthorization(Roles = "Admin")]
    public class TrafficSpeedController : Controller
    {
        private readonly FSPDataContext db = new FSPDataContext();

        //
        // GET: /AdminArea/Freeways/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AdminArea/Freeways/Create

        [HttpPost]
        public ActionResult Create(TrafficSpeed TrafficSpeed)
        {
            if (ModelState.IsValid)
            {
                if (!db.TrafficSpeeds.Any(p => p.TrafficSpeed1 == TrafficSpeed.TrafficSpeed1))
                {
                    //if TrafficSpeed does not exist yet       
                    TrafficSpeed.TrafficSpeedID = Guid.NewGuid();
                    db.TrafficSpeeds.InsertOnSubmit(TrafficSpeed);
                    db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }

            return View(TrafficSpeed);
        }

        //
        // GET: /AdminArea/Freeways/Delete/5

        public ActionResult Delete(Guid id)
        {
            var TrafficSpeed = db.TrafficSpeeds.Single(r => r.TrafficSpeedID == id);
            if (TrafficSpeed == null) return HttpNotFound();
            return View(TrafficSpeed);
        }

        //
        // POST: /AdminArea/Freeways/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var TrafficSpeed = db.TrafficSpeeds.Single(r => r.TrafficSpeedID == id);
            db.TrafficSpeeds.DeleteOnSubmit(TrafficSpeed);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /AdminArea/Freeways/Details/5

        //public ActionResult Details(Guid id)
        //{
        //    var TrafficSpeed = db.TrafficSpeeds.Single(r => r.TrafficSpeedID == id);
        //    if (TrafficSpeed == null) return HttpNotFound();
        //    return View(TrafficSpeed);
        //}

        //
        // GET: /AdminArea/Freeways/Edit/5

        public ActionResult Edit(Guid id)
        {
            var TrafficSpeed = db.TrafficSpeeds.Single(r => r.TrafficSpeedID == id);
            if (TrafficSpeed == null) return HttpNotFound();
            return View(TrafficSpeed);
        }

        //
        // POST: /AdminArea/Freeways/Edit/5

        [HttpPost]
        public ActionResult Edit(TrafficSpeed TrafficSpeed)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TrafficSpeeds.Attach(TrafficSpeed);
                    db.Refresh(RefreshMode.KeepCurrentValues, TrafficSpeed);
                    db.SubmitChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return View(TrafficSpeed);
        }

        //
        // GET: /AdminArea/Freeways/

        public ActionResult Index()
        {
            return View(db.TrafficSpeeds.OrderBy(p => p.TrafficSpeed1).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}