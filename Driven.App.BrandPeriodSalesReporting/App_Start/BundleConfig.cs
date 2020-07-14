using System.Web;
using System.Web.Optimization;

namespace Driven.App.BrandPeriodSalesReporting
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/scripts/jquery.unobtrusive-ajax.js",
                "~/scripts/jquery.validate.min.js",
                "~/scripts/jquery.validate.unobtrusive.min.js"
            ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/respond.js",
                "~/scripts/bootbox/bootbox.min.js",
                "~/scripts/select2-4.0.1/js/select2.min.js",
                "~/scripts/bootstrap-growl/jquery.bootstrap-growl.min.js",
                "~/scripts/moment-2.9.0/moment.min.js",
                "~/scripts/bootstrap-datetime/js/bootstrap-datetimepicker.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreporting").Include(
                "~/Scripts/app/common.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingbrandperiodsales").Include(
                "~/Scripts/app/brandperiodsales/toplines.js",
                "~/Scripts/app/brandperiodsales/edittopline.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingnotification").Include(
               "~/Scripts/app/notification/notifications.js",
                "~/Scripts/app/notification/editbrandnotification.js",
                "~/Scripts/app/notification/editlevelnotification.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingsalescorrectionsubmitcorrections").Include(
                "~/Scripts/app/salescorrection/submitcorrections.js",
                "~/Scripts/app/salescorrection/createtoplinecorrection.js",
                "~/Scripts/app/salescorrection/edittoplinecorrection.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingsalescorrectionapprovecorrections").Include(
                "~/Scripts/app/salescorrection/approvecorrections.js",
                "~/Scripts/app/salescorrection/approvetoplinecorrection.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingcorrectiontype").Include(
                "~/Scripts/app/correctiontype/correctiontypes.js",
                "~/Scripts/app/correctiontype/createcorrectiontype.js",
                "~/Scripts/app/correctiontype/editcorrectiontype.js"
            ));

            
            bundles.Add(new ScriptBundle("~/bundles/drivenappbrandperiodsalesreportingdenialtype").Include(
                "~/Scripts/app/denialtype/denialtypes.js",
                "~/Scripts/app/denialtype/createdenialtype.js",
                "~/Scripts/app/denialtype/editdenialtype.js"
            ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/scripts/select2-4.0.1/css/select2.min.css",
                "~/scripts/select2-4.0.1/css/select2-bootstrap.css",
                "~/Content/bootstrap-datetimepicker.min.css",
                "~/Content/site.css"
            ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862 
            BundleTable.EnableOptimizations = !HttpContext.Current.IsDebuggingEnabled;
        }
    }
}
