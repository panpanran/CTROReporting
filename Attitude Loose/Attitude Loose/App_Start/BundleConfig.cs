using System.Web;
using System.Web.Optimization;

namespace Attitude_Loose
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/CSS").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/CustomStyles.css",
                    "~/Content/themes/base/jquery-ui.css"
                     ));

            bundles.Add(new ScriptBundle("~/Scripts/HomeLayout").Include(
                   "~/Scripts/jquery-2.1.1.js",
                   "~/Scripts/jquery.unobtrusive-ajax.js",
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/jquery-ui-1.12.1.js",
                   "~/Scripts/jquery.nicescroll.js"//modal
                   ));

            bundles.Add(new ScriptBundle("~/Scripts/PageLayout").Include(
                   "~/Scripts/jquery-2.1.1.js",
                   "~/Scripts/jquery.unobtrusive-ajax.js",
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/jquery-ui-1.12.1.js",//datepicker
                   "~/Scripts/jquery.nicescroll.js"//resize image modal
          ));

        }
    }
}
