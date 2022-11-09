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
                        "~/Content/script/js/tsalx.js",
                        "~/Content/script/js/equipe.js"
                        ) );

            bundles.Add( new StyleBundle( "~/Content/css" )
                         .Include( "~/Content/css/tema.css", 
                                   "~/Content/css/tsalx.css",
                                   "~/Content/css/select2-bootstrap.min.css" ) );
        }
    }
}