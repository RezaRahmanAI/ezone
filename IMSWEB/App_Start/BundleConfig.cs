using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace IMSWEB
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.VirtualPathProvider = new ScriptBundlePathProvider(HostingEnvironment.VirtualPathProvider);
            AddAppBundle(bundles);
            BundleTable.EnableOptimizations = false;
        }

        private static void AddAppBundle(BundleCollection bundles)
        {
            var preDOMScriptBundle = new ScriptBundle("~/IMSPreDOMScripts");
            var postDOMScriptBundle = new ScriptBundle("~/IMSPostDOMScripts");
            var styleBundle = new ScriptBundle("~/IMSStyles");
            var scriptDir = "Scripts";
            var styleDir = "Content";
            string SidebarDir = "SideBar";
            var adminAppDirFullPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}", scriptDir));

            if (Directory.Exists(adminAppDirFullPath))
            {
                preDOMScriptBundle.Include(
                    string.Format("~/{0}/jquery.js", scriptDir),
                    string.Format("~/{0}/jquery-ui.js", scriptDir),
                    string.Format("~/{0}/jquery.ui.autocomplete.scroll.js", scriptDir),
                    string.Format("~/{0}/fusion-chart/fusioncharts.js", scriptDir),
                    string.Format("~/{0}/fusion-chart/fusioncharts.charts.js", scriptDir),
                    string.Format("~/{0}/fusion-chart/fusioncharts.theme.fint.js", scriptDir),
                    string.Format("~/{0}/fusion-chart/fusioncharts-jquery-plugin.js", scriptDir),
                    string.Format("~/{0}/toastr/toastr.js", scriptDir),
                    string.Format("~/{0}/data-table/bootstrap-table.min.js", scriptDir),
                    string.Format("~/{0}/data-table/tableExport.js", scriptDir),
                    string.Format("~/{0}/data-table/bootstrap-table-export.js", scriptDir),
                    string.Format("~/{0}/datetime-picker/moment-with-locales.min.js", scriptDir),
                    string.Format("~/{0}/datetime-picker/bootstrap-datetimepicker.min.js", scriptDir),
                    string.Format("~/{0}/select2/select2.min.js", scriptDir));

                postDOMScriptBundle.Include(
                    string.Format("~/{0}/jquery.validate*", scriptDir),
                    string.Format("~/{0}/jquery.unobtrusive-ajax.min.js", scriptDir),
                    string.Format("~/{0}/modernizr-*", scriptDir),
                    string.Format("~/{0}/bootstrap.js", scriptDir),
                    string.Format("~/{0}/respond.js", scriptDir),
                    string.Format("~/{0}/site.js", scriptDir),
                    string.Format("~/{0}/js/SideBar.js", SidebarDir));
            }

            adminAppDirFullPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}", styleDir));
            if (Directory.Exists(adminAppDirFullPath))
            {
                styleBundle.Include(
                    string.Format("~/{0}/bootstrap.css", styleDir),
                    string.Format("~/{0}/home-dashboard.css", styleDir),
                    string.Format("~/{0}/font-awesome.min.css", styleDir),
                    string.Format("~/{0}/toastr/toastr.css", styleDir),
                    string.Format("~/{0}/data-table/bootstrap-table.min.css", styleDir),
                    string.Format("~/{0}/datetime-picker/bootstrap-datetimepicker.min.css", styleDir),
                    string.Format("~/{0}/select2/select2.min.css", styleDir),
                    string.Format("~/{0}/select2/select2-bootstrap.min.css", styleDir),
                    string.Format("~/{0}/site.css", styleDir),
                     string.Format("~/{0}/css/SideBar.css", SidebarDir)
                    );
            }

            bundles.Add(preDOMScriptBundle);
            bundles.Add(postDOMScriptBundle);
            bundles.Add(styleBundle);
        }
    }

    class ScriptBundlePathProvider : VirtualPathProvider
    {
        private readonly VirtualPathProvider _virtualPathProvider;

        public ScriptBundlePathProvider(VirtualPathProvider virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }

        public override bool FileExists(string virtualPath)
        {
            return _virtualPathProvider.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return _virtualPathProvider.GetFile(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return _virtualPathProvider.GetDirectory(virtualDir);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return _virtualPathProvider.DirectoryExists(virtualDir);
        }
    }
}
