using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace TSALX
{
    public class BundleConfig
    {
        public static void RegisterBundles( BundleCollection  bundles )
        {
            bundles.Add( new StyleBundle( "~/Content/css" )
                         .Include( "~/Content/css/tema.css", "~/Content/css/tsalx.css" ) );
        }
    }
}