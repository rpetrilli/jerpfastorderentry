using System.Web;
using System.Web.Optimization;

namespace fastOrderEntry
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Utilizzare la versione di sviluppo di Modernizr per eseguire attività di sviluppo e formazione. Successivamente, quando si è
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-submenu.js",
                      "~/Scripts/bootstrap3-typeahead.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/shortcut.js",
                      "~/Scripts/config-template.js"));


            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                        "~/Scripts/angular/angular.js",
                        "~/Scripts/angular/angular-locale_it-it.js",
                        "~/Scripts/angular/angular-resource.js",
                        "~/Scripts/angular/dirPagination.js",
                        "~/Scripts/angular/webCore.js",
                        "~/Scripts/angular/appModules.js",
                        "~/Scripts/angular/appServices.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/bootstrap-submenu.css",
                      "~/Content/font-awesome.css",
                       "~/Content/admin.css",
                      "~/Content/custom.css"));
        }
    }
}
