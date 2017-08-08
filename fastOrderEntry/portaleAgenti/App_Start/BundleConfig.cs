using System.Web;
using System.Web.Optimization;

namespace portaleAgenti
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilizzare la versione di sviluppo di Modernizr per eseguire attività di sviluppo e formazione. Successivamente, quando si è
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-price-slider.js",
                      "~/Scripts/jquery.collapse.js",
                      "~/Scripts/jquery.fancybox.js",
                      "~/Scripts/jquery.bxslider.js",
                       "~/Scripts/owl.carousel.js",
                      "~/Scripts/jquery.meanmenu.js",
                      "~/Scripts/jquery.scrollUp.js",
                      "~/Scripts/plugins.js",
                      "~/Scripts/wow.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                     "~/Scripts/angular/angular.js",
                     "~/Scripts/angular/angular-locale_it.js",
                     "~/Scripts/angular/dirPagination.js",
                     "~/Scripts/angular/customFilters.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/normalize.css",
                      "~/Content/bootstrap.css",
                      "~/Content/animate.css",
                      "~/Content/font-awesome.css",
                      "~/Content/jquery.ui.css",
                      "~/Content/jquery.bxslider.css",
                      "~/Content/jquery.fancybox.css",
                      "~/Content/meanmenu.css",
                      "~/Content/owl.carousel.css",
                      "~/Content/owl.theme.css",
                      "~/Content/owl.transition.css",
                      "~/Content/pe-icon-7.css",
                      "~/Content/responsive.css",
                      "~/Content/main.css"));
        }
    }
}
