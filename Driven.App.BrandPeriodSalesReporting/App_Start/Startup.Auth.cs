//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using Okta.AspNet;
//using Owin;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Net;

//namespace Driven.App.BrandPeriodSalesReporting
//{
//    public partial class Startup
//    {
//        private const string mc_Configuration_AuthType = "AuthType";
//        private const string mc_Configuration_AuthCookieName = "AuthCookieName";
//        private const string mc_Configuration_AuthLoginPath = "AuthLoginPath";
//        private const string mc_Configuration_AuthExpireTimeSpan = "AuthExpireTimeSpan";


//        public void ConfigureAuth(IAppBuilder app)
//        {
//            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

//            string authType = ConfigurationManager.AppSettings[mc_Configuration_AuthType];
//            string authCookieName = ConfigurationManager.AppSettings[mc_Configuration_AuthCookieName];
//            //string authLoginPath = ConfigurationManager.AppSettings[mc_Configuration_AuthLoginPath];
//            double authExpireTimeSpan = Convert.ToDouble(ConfigurationManager.AppSettings[mc_Configuration_AuthExpireTimeSpan]);

//            app.SetDefaultSignInAsAuthenticationType(OktaDefaults.MvcAuthenticationType);
//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = authType,
//                //LoginPath = new PathString(authLoginPath),
//                Provider = new CookieAuthenticationProvider(),
//                CookieName = authCookieName,
//                CookieHttpOnly = true,
//                ExpireTimeSpan = TimeSpan.FromMinutes(authExpireTimeSpan),
//            });

//            app.UseOktaMvc(new OktaMvcOptions()
//            {
//                OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"],
//                ClientId = ConfigurationManager.AppSettings["okta:ClientId"],
//                ClientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"],
//                RedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"],
//                PostLogoutRedirectUri = ConfigurationManager.AppSettings["okta:PostLogoutRedirectUri"],
//                GetClaimsFromUserInfoEndpoint = true,
//                Scope = new List<string> { "openid", "profile", "email" },
//            });


//        }
//    }
//}
