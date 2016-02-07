using System.Web;
using System.Web.Optimization;

namespace Website
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/j").Include(
            "~/Scripts/script.js"));

            bundles.Add(new StyleBundle("~/bundles/c").Include(
                "~/Content/normalize.css",
                "~/Content/skeleton.css"));
        }
    }
}