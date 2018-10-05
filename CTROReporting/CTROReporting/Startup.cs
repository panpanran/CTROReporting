using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute("CTROReportingConfig", typeof(CTROReporting.Startup))]
namespace CTROReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseHangfireDashboard("/ctroreporting");
        }
    }
}
