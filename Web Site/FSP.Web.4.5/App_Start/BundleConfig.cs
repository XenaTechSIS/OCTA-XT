using System.Web.Optimization;

namespace FSP.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/appAngular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/angular-ui/ui-bootstrap.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                "~/Scripts/googleMap/jquery.minicolors.min.js",
                "~/Scripts/googleMap/angular-minicolors.js",
                "~/Scripts/googleMap/ngMask.min.js",
                "~/Scripts/googleMap/checklist-model.js",
                "~/app/app.js",
                "~/app/core/*.js",
                "~/app/filters/*.js",
                "~/app/services/*.js",
                "~/app/map/map.module.js",
                "~/app/map/*.js",
                "~/app/map/directives/*.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-{version}"));


            //jqueryVal
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            //jquery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            //jqueryUI
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            //bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap-3.3.2.js"
            ));

            //app
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/sortable.js",
                "~/Scripts/toastr.js",
                "~/Scripts/moment.js"
            ));


            //KO + signalR
            bundles.Add(new ScriptBundle("~/bundles/KOAndSignarlR").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/jquery.signalR-{version}.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/KO").Include(
                "~/Scripts/knockout-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fspDispatch").Include(
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.dispatchViewModel.js",
                "~/MyScripts/fsp.truckCollection.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fspAlertMessages").Include(
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.alertMessagesViewModel.js",
                "~/MyScripts/fsp.truckCollection.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fspAlarmHistory").Include(
                "~/Scripts/jquery.ui-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/moment.js",
                "~/MyScripts/alarmHistory.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fspDriversAlertComments").Include(
                "~/Scripts/jquery.ui-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/moment.js",
                "~/MyScripts/driversAlertComments.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/alertsViewModel").Include(
                "~/Scripts/moment.js",
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.alertsViewModel.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fspTruckList").Include(
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.gridViewModel.js",
                "~/MyScripts/fsp.truckCollection.js"
            ));


            //assist
            bundles.Add(new ScriptBundle("~/bundles/fspAssist").Include(
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.assistViewModel.js"
            ));

            //incident
            bundles.Add(new ScriptBundle("~/bundles/fspIncident").Include(
                "~/MyScripts/fsp.constructor.js",
                "~/MyScripts/fsp.incidentViewModel.js"
            ));


            //css & styles

            bundles.Add(new StyleBundle("~/bundles/octa").Include(
                "~/Content/themes/base/all.css",
                "~/Content/bootstrap-3.3.2.css",
                "~/Content/font-awesome.css",
                "~/Content/toastr.css",
                "~/Content/titatoggle-dist-min.css",
                "~/Content/jquery.minicolors.css",
                "~/Content/octa/bootstrap-custom.css",
                "~/Content/octa/core.css"
            ));

            bundles.Add(new StyleBundle("~/Content/jqueryUI").Include(
                "~/Content/themes/base/all.css"));


            BundleTable.EnableOptimizations = false;
        }
    }
}