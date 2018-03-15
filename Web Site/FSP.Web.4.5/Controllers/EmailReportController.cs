using System;
using System.Data.Entity;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;

namespace FSP.Web.Controllers
{
    [CustomAuthorization(Roles = "Admin")]
    public class EmailReportController : Controller
    {
        private readonly FSPDataContext db = new FSPDataContext();

        //
        // GET: /Junk/Create

        public ActionResult Create()
        {
            var users = from q in db.Users
                select new
                {
                    q.Email,
                    FullName = q.FirstName + " " + q.LastName
                };

            ViewBag.AERecipientEmail = new SelectList(users.OrderBy(p => p.FullName), "Email", "FullName");
            ViewBag.AEFrequencyID = new SelectList(db.AEFrequencies.OrderBy(p => p.AEFrequencyName), "AEFrequencyID",
                "AEFrequencyName");
            ViewBag.AEReportID =
                new SelectList(db.AEReports.OrderBy(p => p.AEReportName), "AEReportID", "AEReportName");
            ViewBag.AEReportTypeID = new SelectList(db.AEReportTypes.OrderBy(p => p.AEReportTypeName), "AEReportTypeID",
                "AEReportTypeName");
            ViewBag.AEContractorID = new SelectList(db.Contractors.OrderBy(p => p.ContractCompanyName), "ContractorID",
                "ContractCompanyName");
            return View();
        }

        //
        // POST: /Junk/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AERecipient aerecipient)
        {
            if (ModelState.IsValid)
            {
                aerecipient.AERecipientID = Guid.NewGuid();
                if (aerecipient.AEContractorID != null)
                    aerecipient.AEIsContractor = true;
                else
                    aerecipient.AEIsContractor = false;

                db.AERecipients.InsertOnSubmit(aerecipient);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var users = from q in db.Users
                select new
                {
                    q.Email,
                    FullName = q.FirstName + " " + q.LastName
                };

            ViewBag.AERecipientEmail = new SelectList(users.OrderBy(p => p.FullName), "Email", "FullName");

            ViewBag.AEFrequencyID = new SelectList(db.AEFrequencies.OrderBy(p => p.AEFrequencyName), "AEFrequencyID",
                "AEFrequencyName", aerecipient.AEFrequencyID);
            ViewBag.AEReportID = new SelectList(db.AEReports.OrderBy(p => p.AEReportName), "AEReportID", "AEReportName",
                aerecipient.AEReportID);
            ViewBag.AEReportTypeID = new SelectList(db.AEReportTypes.OrderBy(p => p.AEReportTypeName), "AEReportTypeID",
                "AEReportTypeName", aerecipient.AEReportTypeID);
            ViewBag.AEContractorID = new SelectList(db.Contractors.OrderBy(p => p.ContractCompanyName), "ContractorID",
                "ContractCompanyName", aerecipient.AEContractorID);
            return View(aerecipient);
        }

        //
        // GET: /Junk/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            var aerecipient = db.AERecipients.FirstOrDefault(p => p.AERecipientID == id);
            if (aerecipient == null) return HttpNotFound();
            return View(aerecipient);
        }

        //
        // POST: /Junk/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var aerecipient = db.AERecipients.FirstOrDefault(p => p.AERecipientID == id);
            db.AERecipients.DeleteOnSubmit(aerecipient);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Junk/Details/5

        //public ActionResult Details(Guid? id = null)
        //{
        //    var aerecipient = db.AERecipients.FirstOrDefault(p => p.AERecipientID == id);
        //    if (aerecipient == null) return HttpNotFound();
        //    return View(aerecipient);
        //}

        //
        // GET: /Junk/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            var aerecipient = db.AERecipients.FirstOrDefault(p => p.AERecipientID == id);
            if (aerecipient == null) return HttpNotFound();
            ViewBag.AEFrequencyID = new SelectList(db.AEFrequencies.OrderBy(p => p.AEFrequencyName), "AEFrequencyID",
                "AEFrequencyName", aerecipient.AEFrequencyID);
            ViewBag.AEReportID = new SelectList(db.AEReports.OrderBy(p => p.AEReportName), "AEReportID", "AEReportName",
                aerecipient.AEReportID);
            ViewBag.AEReportTypeID = new SelectList(db.AEReportTypes.OrderBy(p => p.AEReportTypeName), "AEReportTypeID",
                "AEReportTypeName", aerecipient.AEReportTypeID);
            ViewBag.AEContractorID = new SelectList(db.Contractors.OrderBy(p => p.ContractCompanyName), "ContractorID",
                "ContractCompanyName", aerecipient.AEContractorID);
            return View(aerecipient);
        }

        //
        // POST: /Junk/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AERecipient aerecipient)
        {
            if (ModelState.IsValid)
            {
                if (aerecipient.AEContractorID != null)
                    aerecipient.AEIsContractor = true;
                else
                    aerecipient.AEIsContractor = false;

                db.AERecipients.Attach(aerecipient);
                db.Refresh(RefreshMode.KeepCurrentValues, aerecipient);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AEFrequencyID = new SelectList(db.AEFrequencies.OrderBy(p => p.AEFrequencyName), "AEFrequencyID",
                "AEFrequencyName", aerecipient.AEFrequencyID);
            ViewBag.AEReportID = new SelectList(db.AEReports.OrderBy(p => p.AEReportName), "AEReportID", "AEReportName",
                aerecipient.AEReportID);
            ViewBag.AEReportTypeID = new SelectList(db.AEReportTypes.OrderBy(p => p.AEReportTypeName), "AEReportTypeID",
                "AEReportTypeName", aerecipient.AEReportTypeID);
            ViewBag.AEContractorID = new SelectList(db.Contractors.OrderBy(p => p.ContractCompanyName), "ContractorID",
                "ContractCompanyName", aerecipient.AEContractorID);
            return View(aerecipient);
        }

        //
        // GET: /Junk/

        public ActionResult Index()
        {
            var aerecipients = db.AERecipients.Include(a => a.AEFrequency).Include(a => a.AEReport)
                .Include(a => a.AEReportType).OrderBy(p => p.AERecipientEmail);
            return View(aerecipients.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}