using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

using Driven.Business.Security;
using Driven.Business.Security.Interfaces;
using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;
using Okta.AspNet;

namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    public class LoginController : Controller
    {
        //private IAuthenticationService _authenticationService;

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //public LoginController(IAuthenticationService authenticationService)
        //{
        //    _authenticationService = authenticationService;
        //}


        //[AllowAnonymous]
        //public virtual ActionResult Index()
        //{
        //    if (!HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        HttpContext.GetOwinContext().Authentication.Challenge(
        //            OktaDefaults.MvcAuthenticationType);
        //        return new HttpUnauthorizedResult();
        //    }
        //    return RedirectUser();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public virtual ActionResult Index(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var authenticationResult = this._authenticationService.SignIn(model.Username, model.Password);
        //    var authenticationMessage = authenticationResult.ErrorMessage;

        //    if (authenticationResult.IsSuccess)
        //    {
        //        var appGroups = SecurityHelpers.GetApplicationGroups();
        //        if (appGroups != null && appGroups.Count > 0)
        //        {
        //            return RedirectToLocal(returnUrl);
        //        }
        //        else
        //        {
        //            Session.Abandon();
        //            _authenticationService.SignOut();
        //            authenticationMessage = "You are not a member of a required application group.";
        //        }
        //    }

        //    ModelState.AddModelError("", authenticationMessage);
        //    return View(model);
        //}

        [Authorize]
        private ActionResult RedirectUser()
        {
            return new ViewResult();
        }

        public ActionResult Logoff()
        {
            Session.Abandon();
            //_authenticationService.SignOut();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext()
                           .Authentication
                           .SignOut(OktaDefaults.MvcAuthenticationType);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SwitchSubBrand(int? subBrandId)
        {
            if (subBrandId > 0)
            {
                SecurityHelpers.SetUserSubBrandId((int)subBrandId);
            }
            else
            {
                Session.Abandon();
                //_authenticationService.SignOut();
                //if (HttpContext.User.Identity.IsAuthenticated)
                //{
                //    HttpContext.GetOwinContext()
                //               .Authentication
                //               .SignOut(OktaDefaults.MvcAuthenticationType);
                //}
                return RedirectToAction("Logout", "Saml2");
            }

            return RedirectToDefault();
        }


        private RedirectToRouteResult RedirectToDefault()
        {
            return RedirectToAction("", "BrandPeriodSales");
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToDefault();
        }

    }
}
