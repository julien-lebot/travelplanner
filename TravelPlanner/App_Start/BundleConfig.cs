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
                "~/public/lib/jquery/jquery-{version}.js");

            var angularJs = new ScriptBundle("~/bundles/angular", "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.5/angular.min.js").Include(
                "~/public/lib/angularjs/angular.js");

            var angularRoute = new ScriptBundle("~/bundles/angular-route", "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.5/angular-route.js").Include(
                "~/public/lib/angular-route/angular-route.js");

            var mdlDesignCss = new StyleBundle("~/bundles/css/mdldesignlite").Include(
                "~/public/lib/mdldesignlite/material.css"
                );

            var mdljQueryModalCss = new StyleBundle("~/bundles/css/mdljQueryModal").Include(
                "~/public/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.css"
                );

            var mdlDesignJs = new ScriptBundle("~/bundles/mdldesignlite").Include(
                "~/public/lib/mdldesignlite/material.js"
                );

            var mdljQueryModalJs = new ScriptBundle("~/bundles/mdljQueryModal").Include(
                "~/public/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.js"
                );

            var momentJs = new ScriptBundle("~/bundles/momentjs").Include(
                "~/public/lib/momentjs/moment-with-locales.js"
                );

            var appJs = new ScriptBundle("~/bundles/app").Include(
                "~/public/js/app/app.js",
                "~/public/js/app/controllers/*.js",
                "~/public/js/app/services/*.js",
                "~/public/js/app/filters/*.js",
                "~/public/js/app/directives/*.js"
                );

            var appCss = new StyleBundle("~/bundles/css").Include(
                "~/public/css/*.css");

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
