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
using System.Runtime.ExceptionServices;
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

            CTROConst st = new CTROConst();
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;
            //if (!CTROFunctions.ConnectSSHCTRP())
            //{
            //    Logging.WriteLog("Host", "ConnectSSHCTRP", "Data warehouse connecting Wrong or has been connected!");
            //}
        }

        static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine("FirstChanceException event raised in {0}: {1}",
                AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
        }
    }
}
