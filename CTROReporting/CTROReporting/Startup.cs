using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("CTROReportingConfig", typeof(CTROReporting.Startup))]
namespace CTROReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
