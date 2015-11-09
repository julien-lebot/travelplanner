using System.Web;
using System.Web.Optimization;

namespace TravelPlanner
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var jQuery = new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/lib/jquery/*.js");

            var angularJs = new ScriptBundle("~/bundles/angular").Include(
                "~/Content/lib/angularjs/*.js");

            var angularRoute = new ScriptBundle("~/bundles/angular-route").Include(
                "~/Content/lib/angular-route/*.js");

            var mdlDesignCss = new StyleBundle("~/bundles/css/mdldesignlite").Include(
                "~/Content/lib/mdldesignlite/*.css"
                );

            var mdljQueryModalCss = new StyleBundle("~/bundles/css/mdljQueryModal").Include(
                "~/Content/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.css"
                );

            var mdlDesignJs = new ScriptBundle("~/bundles/mdldesignlite").Include(
                "~/Content/lib/mdldesignlite/*.js"
                );

            var mdljQueryModalJs = new ScriptBundle("~/bundles/mdljQueryModal").Include(
                "~/Content/lib/mdl-jquery-modal-dialog/mdl-jquery-modal-dialog.js"
                );

            var momentJs = new ScriptBundle("~/bundles/momentjs").Include(
                "~/Content/lib/momentjs/moment-with-locales.js"
                );

            var appJs = new ScriptBundle("~/bundles/app").Include(
                "~/Content/js/app/app.js",
                "~/Content/js/app/services/*.js",
                "~/Content/js/app/filters/*.js",
                "~/Content/js/app/controllers/*.js",
                "~/Content/js/app/directives/*.js"
                );

            var appCss = new StyleBundle("~/bundles/css").Include(
                "~/Content/css/*.css");

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
