using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;

using Driven.Business.Royalty;
using Driven.Data.Entity.Royalty;
using Driven.App.BrandPeriodSalesReporting.Models;


namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class SecurityHelpers
    {
        private const string mc_Configuration_AuthFake = "AuthFake";
        private const string mc_Configuration_AuthFakeGroups = "AuthFakeGroups";
        private const string mc_Configuration_AuthDomainName = "AuthDomainName";

        private const string mc_Session_Current_UserViewModel = "SESSION_CURRENT_USERVIEWMODEL";
        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static MvcHtmlString AntiForgeryTokenForAjaxPost(this HtmlHelper helper)
        {
            var antiForgeryInputTag = helper.AntiForgeryToken().ToString();
            var removedStart = antiForgeryInputTag.Replace(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""", "");
            var tokenValue = removedStart.Replace(@""" />", "");

            if (antiForgeryInputTag == removedStart || removedStart == tokenValue)
            {
                throw new InvalidOperationException("Html.AntiForgeryToken() method threw exception.");
            }

            return new MvcHtmlString(string.Format(@"{0}:""{1}""", "__RequestVerificationToken", tokenValue));
        }


        public static IEnumerable<Claim> GetUserClaims()
        {
            try
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                _log.Info(identity.Claims);
                return identity.Claims;
            }
            catch (Exception ex)
            {
                _log.Warn(FormatException(ex));
                return null;
            }
        }


        //private static UserPrincipal GetUserPrincipal(string userName)
        //{
        //    try
        //    {
        //        var domain = new PrincipalContext(ContextType.Machine);
        //        return UserPrincipal.FindByIdentity(domain, userName);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Warn(FormatException(ex));
        //        var domainName = GetDomainName();
        //        return GetUserPrincipal(userName, domainName);
        //    }
        //}


        //private static UserPrincipal GetUserPrincipal(string userName, string domainName)
        //{
        //    var domain = new PrincipalContext(ContextType.Domain, domainName);
        //    return UserPrincipal.FindByIdentity(domain, userName);
        //}


        //public static string GetDisplayName(string userName)
        //{
        //    try
        //    {
        //        return GetUserPrincipal(userName).DisplayName;
        //    }
        //    catch(Exception ex)
        //    {
        //        _log.Warn(FormatException(ex));
        //        return string.Empty;
        //    }
        //}


        //public static string GetEmailAddress(string userName)
        //{
        //    try
        //    {
        //        return GetUserPrincipal(userName).EmailAddress;
        //    }
        //    catch(Exception ex)
        //    {
        //        _log.Warn(FormatException(ex));
        //        return string.Empty;
        //    }
        //}


        public static List<string> GetGroupEmailAddresses(string groupName)
        {
            var emailList = new List<string>();

            try
            {
                var domainName = GetDomainName();
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    _log.InfoFormat("Domain {0}, Group {1} Emails...", domainName, groupName);
                    using (var group = GroupPrincipal.FindByIdentity(context, groupName))
                    {
                        if (group != null)
                        {
                            var users = group.GetMembers(true);
                            foreach (UserPrincipal user in users)
                            {
                                _log.InfoFormat("Domain {0}, Group {1}, User: {2}, Email: {3}", domainName, groupName, user.Name, user.EmailAddress);
                                emailList.Add(user.EmailAddress);
                            }
                        }
                        else 
                        {
                            _log.InfoFormat("Group {0} Null", groupName);
                        }
                    }
                }

                return emailList;
            }
            catch(Exception ex)
            {
                _log.Warn(FormatException(ex));
                return emailList;
            }
        }


        //public static List<string> GetUserGroups(string userName, string domainName)
        //{
        //    var results = new List<string>();

        //    try
        //    {
        //        var domain = new PrincipalContext(ContextType.Domain, domainName);
        //        var user = UserPrincipal.FindByIdentity(domain, userName);
        //        return user.GetGroups().Select(x => x.Name).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Warn(FormatException(ex));
        //        return results;
        //    }
        //}


        private static string GetDomainName()
        {
            return ConfigurationManager.AppSettings[mc_Configuration_AuthDomainName] ?? string.Empty;
        }


        public static List<string> GetUserGroups()
        {
            try
            {
                if(((ClaimsIdentity)HttpContext.Current.User.Identity).HasClaim(x => x.Type == "Group"))
                {
                    var groups = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Where(x => x.Type == "Group").Select(x => x.Value).ToList();
                    return groups;
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                _log.Warn(FormatException(ex));
                return new List<string>();
            }
        }


        public static List<string> GetSubBrandApproverGroups(int subBrandId, bool IncludeAdminGroups = false)
        {
            var results = new List<string>();
            var brandAuthService = new BrandAuthService();
            var subBrandAuths = (IncludeAdminGroups)
                ? brandAuthService.GetSubBrandAuthGroupXrefs().Where(x => x.SubBrandID == subBrandId && x.Approver == true)
                : brandAuthService.GetSubBrandAuthGroupXrefs().Where(x => x.SubBrandID == subBrandId && x.Approver == true && x.Administrator == false);

            foreach (var sba in subBrandAuths)
            {
                results.Add(sba.AuthGroup);
            }

            return results;
        }


        public static List<BPSR_SubBrandAuth> GetApplicationGroups()
        {
            var domainGroupNames = GetUserGroups();
            foreach (var item in domainGroupNames)
            {
                _log.Info("GroupName : " + item);
            }
            bool reportAccess = false;
            
            if (domainGroupNames.Contains("AWS_SEC_CORP_US_BPSR_QlikView")) {
                reportAccess = true;
            }

            var brandAuthService = new BrandAuthService();
            var subBrandAuths = brandAuthService.GetSubBrandAuthsInAuthGroups(domainGroupNames).OrderBy(x => x.SubBrandName).ToList();

            foreach(BPSR_SubBrandAuth subBrandAuth in subBrandAuths) {
                subBrandAuth.ReportsAvailable = reportAccess;
            }

            return subBrandAuths;
        }


        public static bool IsUserAuthenticated()
        {
            return (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated);
        }


        //public static string GetUserName()
        //{
        //    try
        //    {
        //        return HttpContext.Current.User.Identity.Name;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Warn(FormatException(ex));
        //        return null;
        //    }
        //}

        public static string GetUserName()
        {
            try
            {
                var userClaims = GetUserClaims();
                var email = userClaims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                return email;
            }
            catch (Exception ex)
            {
                _log.Warn(FormatException(ex));
                return null;
            }
        }


        public static UserViewModel SetUserSubBrandId(int subBrandId)
        {
            var model = BuildUserViewModel(subBrandId);

            HttpContext.Current.Session[mc_Session_Current_UserViewModel] = model;

            return model;
        }


        public static UserViewModel GetUserViewModel()
        {
            try
            {
                UserViewModel model = null;

                if (IsUserAuthenticated())
                {
                    var userName = GetUserName();

                    if (HttpContext.Current.Session[mc_Session_Current_UserViewModel] != null && ((UserViewModel)HttpContext.Current.Session[mc_Session_Current_UserViewModel]).Name == userName)
                    {
                        model = (UserViewModel)HttpContext.Current.Session[mc_Session_Current_UserViewModel];
                    }
                    else
                    {
                        model = BuildUserViewModel();
                        HttpContext.Current.Session[mc_Session_Current_UserViewModel] = model;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                _log.Warn(FormatException(ex));
                return null;
            }
        }


        private static UserViewModel BuildUserViewModel()
        {
            return BuildUserViewModel(null);
        }


        private static UserViewModel BuildUserViewModel(int? subBrandId)
        {
            try
            {
                var userName = HttpContext.Current.User.Identity.Name;
                var subBrandAuths = GetApplicationGroups();

                var available = (from t in subBrandAuths group t by new { t.SubBrandID, t.SubBrandName } into grp select new { grp.Key.SubBrandID, grp.Key.SubBrandName }).ToDictionary(x => x.SubBrandID, x => x.SubBrandName);
                var current = (subBrandId != null) ? subBrandAuths.Where(x => x.SubBrandID == subBrandId).FirstOrDefault() : subBrandAuths.FirstOrDefault();
                var userClaims = GetUserClaims();
                foreach (var claim in userClaims)
                {
                    _log.Info("claim.Issuer : " + claim.Issuer);
                    _log.Info("claim.OriginalIssuer : " + claim.OriginalIssuer);
                    _log.Info("claim.Subject : " + claim.Subject);
                    _log.Info("claim.Type : " + claim.Type);
                    _log.Info("claim.Value : " + claim.Value);
                    _log.Info("claim.ValueType : " + claim.ValueType);
                    foreach (var item in claim.Properties)
                    {
                        _log.Info("item.Key : " + item.Key);
                        _log.Info("item.Value : " + item.Value);
                    }
                }
                var email = userClaims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                var usertype = GetUserType();
                var userStores = GetUserAuthStores();

                var model = new UserViewModel()
                {
                    SubBrandId = (current != null) ? (int?)current.SubBrandID : null,
                    IsAdmin = (current != null) ? current.Administrator : false,
                    IsApprover = (current != null) ? current.Approver : false,
                    IsReportsAvailable = (current != null) ? (current.ReportsAvailable ?? false) : false,
                    Name = email,
                    AvailableSubBrands = available,
                    DisplayName = string.IsNullOrWhiteSpace(userName)? email: userName,
                    EmailAddress = email,
                    UserType = usertype,
                    UserAuthStores = userStores
                };

                return model;
            }
            catch (Exception ex)
            {
                _log.Warn(FormatException(ex));
                return null;
            }
        }

        private static List<string> GetUserAuthStores()
        {
            var userName = GetUserName();
            _log.Info("AuthenticationType : " + HttpContext.Current.User.Identity.AuthenticationType);
            _log.Info("IsAuthenticated : " + HttpContext.Current.User.Identity.IsAuthenticated);
            _log.Info("Name : " + HttpContext.Current.User.Identity.Name);
            var userService = new UserService();
            var userAuthStores = userService.UserAuthStoresGet(userName).ToList();
            return userAuthStores;
        }

        private static string GetUserType()
        {
            var userName = GetUserName();
            var userService = new UserService();
            var userType = userService.UserTypeGet(userName).FirstOrDefault();
            return userType;
        }


        public static bool IsValidSubmitterApprover(string submitterName, string approverName)
        {
            if (approverName != null && submitterName != null && approverName.ToLower() == submitterName.ToLower())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        
        public static string FormatException(Exception ex)
        {
            string msg = (ex.InnerException != null) ? ex.Message + "; " + ex.InnerException.Message : ex.Message;
            string trace = (ex.StackTrace != null) ? ex.StackTrace : string.Empty;
            return String.Format("{0} - {1}", msg, trace);
        }

    }
}
