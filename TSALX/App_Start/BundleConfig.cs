using System.Web.Optimization;

namespace TSALX
{
    public class BundleConfig
    {
        public static void RegisterBundles( BundleCollection  bundles )
        {
            bundles.Add( new ScriptBundle( "~/Content/script" ).Include(
                        "~/Content/script/jquery.validate.js",
                        "~/Content/script/jquery.validate.unobtrusive.js",
                        "~/Content/script/tsalx.js" ) );

            bundles.Add( new StyleBundle( "~/Content/css" )
                         .Include( "~/Content/css/tema.css", 
                                   "~/Content/css/tsalx.css" ) );
        }
    }
}