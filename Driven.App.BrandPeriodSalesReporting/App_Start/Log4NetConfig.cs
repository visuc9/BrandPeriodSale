using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Driven.App.BrandPeriodSalesReporting
{
    public class Log4NetConfig
    {
        public static void RegisterLog4net()
        {
            log4net.GlobalContext.Properties["hostname"] = System.Net.Dns.GetHostName();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
