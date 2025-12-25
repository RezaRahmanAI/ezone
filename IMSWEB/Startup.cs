using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using IMSWEB.Service;

[assembly: OwinStartup(typeof(IMSWEB.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace IMSWEB
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.ConfigureAutofac(app);
            AuthConfig.ConfigureAuth(app);
        }
    }
}
