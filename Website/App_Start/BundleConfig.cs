using System.Web;
using System.Web.Optimization;

namespace Website
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
            "~/Scripts/script.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/css/normalize.css",
                "~/Content/css/skeleton.css"));
        }
    }
}