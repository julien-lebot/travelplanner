using System.Web;
using System.Web.Optimization;

namespace TravelPlanner
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var jQuery = new ScriptBundle("~/bundles/jquery", "https://code.jquery.com/jquery-2.1.4.min.js").Include(
                "~/wwwroot/lib/jquery/jquery-{version}.js");

            var angularJs = new ScriptBundle("~/bundles/angular", "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.5/angular.min.js").Include(
                "~/wwwroot/lib/angularjs/angular.js");

            var angularRoute = new ScriptBundle("~/bundles/angular-route", "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.5/angular-route.js").Include(
                "~/wwwroot/lib/angular-route/angular-route.js");

            var mdlDesignCss = new StyleBundle("~/bundles/css/mdldesignlite").Include(
                "~/wwwroot/lib/mdldesignlite/material.css"
                );

            var mdljQueryModalCss = new StyleBundle("~/bundles/css/mdljQueryModal").Include(
                "~/wwwroot/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.css"
                );

            var mdlDesignJs = new ScriptBundle("~/bundles/mdldesignlite").Include(
                "~/wwwroot/lib/mdldesignlite/material.js"
                );

            var mdljQueryModalJs = new ScriptBundle("~/bundles/mdljQueryModal").Include(
                "~/wwwroot/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.js"
                );

            var momentJs = new ScriptBundle("~/bundles/momentjs").Include(
                "~/wwwroot/lib/momentjs/moment-with-locales.js"
                );

            var appJs = new ScriptBundle("~/bundles/app").Include(
                "~/wwwroot/js/app/app.js",
                "~/wwwroot/js/app/controllers/*.js",
                "~/wwwroot/js/app/services/*.js",
                "~/wwwroot/js/app/directives/*.js"
                );

            var appCss = new StyleBundle("~/bundles/css").Include(
                "~/wwwroot/css/*.css");

            bundles.Add(jQuery);
            bundles.Add(angularJs);
            bundles.Add(angularRoute);
            bundles.Add(mdlDesignCss);
            bundles.Add(mdljQueryModalCss);
            bundles.Add(mdlDesignJs);
            bundles.Add(mdljQueryModalJs);
            bundles.Add(momentJs);
            bundles.Add(appJs);
            bundles.Add(appCss);

        }
    }
}
