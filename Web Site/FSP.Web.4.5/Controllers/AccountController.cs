using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FSP.Common;
using FSP.Web.Helpers;
using FSP.Web.Models;

namespace FSP.Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult AwaitingApproval()
        {
            return View();
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // ChangePassword will throw an exception rather
            // than return false in certain failure scenarios.
            bool changePasswordSucceeded;
            try
            {
                var currentUser = Membership.GetUser(User.Identity.Name, true);
                changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
                return RedirectToAction("ChangePasswordSuccess");
            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return Json(new {errors = GetErrorsFromModelState()});

            if (Membership.ValidateUser(model.Email, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                return Json(new {success = true, redirect = returnUrl});
            }

            ModelState.AddModelError("", "The user name or password provided is incorrect.");

            // If we got this far, something failed
            return Json(new {errors = GetErrorsFromModelState()});
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonRegister(RegisterModel model)
        {
            if (!ModelState.IsValid) return Json(new {errors = GetErrorsFromModelState()});

            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.Email, model.Password, model.Email, null, null, true, null,
                out createStatus);
            //((FSPMembershipProvider)Membership.Provider).CreateFSPUser(userId, user.Email, "", RoleID, user.FirstName, user.LastName, user.Address, user.City, user.State, user.Zip, user.PhoneNumber, true, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.Email, false);
                return Json(new {success = true});
            }

            ModelState.AddModelError("", Util.ErrorCodeToString(createStatus));

            // If we got this far, something failed
            return Json(new {errors = GetErrorsFromModelState()});
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return ContextDependentView();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            if (Membership.ValidateUser(model.Email, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("",
                "Either the user name or password provided is incorrect or your account has not been yet approved.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            return ContextDependentView();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = Guid.NewGuid();

            // Attempt to register the user
            MembershipCreateStatus createStatus;

            ((FSPMembershipProvider) Membership.Provider).CreateFSPUser(userId, model.Email, model.Password,
                Guid.Empty, false, out createStatus, model.FirstName, model.LastName);

            if (createStatus == MembershipCreateStatus.Success)
            {
                var subject = "New User Registration";
                var body = string.Empty;
                body += "Hello, " + ConfigurationManager.AppSettings["OCTAAdminName"] + ": </br></br>";
                body += "\"" + model.FirstName + " " + model.LastName +
                        "\" has just registered for the <a href='http://latatrax.com/octafsp/' target='_blank'>OCTA FSP web site</a>. " +
                        "</br></br>";
                body +=
                    "Please, view the <a href='http://latatrax.com/octafsp/AdminArea/Users/' target='_blank'>users page</a> to manage user accounts and access.</br></br></br></br>";
                body += "-LATA Trax Email System";

                Util.SendEmail(subject, body);

                return RedirectToAction("AwaitingApproval", "Account");
            }

            ModelState.AddModelError("", Util.ErrorCodeToString(createStatus));

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult ContextDependentView()
        {
            var actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView();
            }

            ViewBag.FormAction = actionName;
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }
    }
}