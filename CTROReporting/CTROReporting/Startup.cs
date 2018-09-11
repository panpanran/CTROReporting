using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("CTROReportingConfig", typeof(CTRPReporting.Startup))]
namespace CTRPReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
