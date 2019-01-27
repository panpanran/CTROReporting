using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute("CTROReportingConfig", typeof(CTROReporting.Startup))]
//[assembly: OwinStartup(typeof(CTROReporting.Startup))]

namespace CTROReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            app.UseHangfireDashboard("/ctroreporting");
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
