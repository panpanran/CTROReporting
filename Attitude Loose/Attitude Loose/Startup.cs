using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Attitude_Loose.Startup))]
namespace Attitude_Loose
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
