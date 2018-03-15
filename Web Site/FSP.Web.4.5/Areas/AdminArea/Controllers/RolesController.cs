using System;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;

namespace FSP.Web.Areas.AdminArea.Controllers
{
    [CustomAuthorization(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly FSPDataContext db = new FSPDataContext();
       
        public ActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                if (db.Roles.Any(p => p.RoleName == role.RoleName) == false)
                {
                    //if role does not exist yet
                    role.RoleID = Guid.NewGuid();
                    db.Roles.InsertOnSubmit(role);
                    db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }

            return View(role);
        }

        public ActionResult Delete(Guid id)
        {
            var role = db.Roles.Single(r => r.RoleID == id);
            if (role == null) return HttpNotFound();
            return View(role);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var role = db.Roles.Single(r => r.RoleID == id);
            db.Roles.DeleteOnSubmit(role);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Edit(Guid id)
        {
            var role = db.Roles.Single(r => r.RoleID == id);
            if (role == null) return HttpNotFound();
            return View(role);
        }

        
        [HttpPost]
        public ActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                if (db.Roles.Any(p => p.RoleName == role.RoleName) == false)
                {
                    db.Roles.Attach(role);
                    db.Refresh(RefreshMode.KeepCurrentValues, role);
                    db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }

            return View(role);
        }
        
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}