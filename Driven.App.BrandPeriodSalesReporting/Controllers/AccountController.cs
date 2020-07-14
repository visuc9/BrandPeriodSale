//using Okta.AspNet;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Driven.App.BrandPeriodSalesReporting.Controllers
//{
//    public class AccountController : Controller
//    {
//        [AllowAnonymous]
//        public virtual ActionResult Index()
//        {
//            return RedirectToAction("SignIn");
//        }
//        [AllowAnonymous]
//        public ActionResult SignIn()
//        {
//            if (!HttpContext.User.Identity.IsAuthenticated)
//            {
//                HttpContext.GetOwinContext().Authentication.Challenge(
//                    OktaDefaults.MvcAuthenticationType);
//                return new HttpUnauthorizedResult();
//            }
//            return RedirectToAction("Index", "BrandPeriodSales");
//        }

//        [AllowAnonymous]
//        public ActionResult SignOut()
//        {
//            Session.Abandon();
//            //_authenticationService.SignOut();
//            if (HttpContext.User.Identity.IsAuthenticated)
//            {
//                HttpContext.GetOwinContext()
//                           .Authentication
//                           .SignOut(OktaDefaults.MvcAuthenticationType);
//            }
//            return RedirectToAction("SignIn");
//        }

//        private ActionResult RedirectUser()
//        {
//            //var isFranchiseUser = HttpContext.GetOwinContext().Authentication.User.HasClaim("FIRMGroup", "AWS_SEC_FIRM_Franchise_User");// .Select(x => x.Type == "FIRMGroup");
//            //if (isFranchiseUser)
//            //{
//            //    return RedirectToAction("Index", "FranchiseUserSales");
//            //}
//            return RedirectToAction("Index", "BrandPeriodSales");
//        }
//	}
//}