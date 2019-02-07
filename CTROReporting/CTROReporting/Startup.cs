using CTROLibrary;
using CTROLibrary.CTRO;
using CTROLibrary.EW;
using Hangfire;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartupAttribute("CTROReportingConfig", typeof(CTROReporting.Startup))]
//[assembly: OwinStartup(typeof(CTROReporting.Startup))]

namespace CTROReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);
            app.UseHangfireDashboard("/ctroreporting");
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
