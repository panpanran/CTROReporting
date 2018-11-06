using CTROLibrary;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Configuration;
using System.Reflection;

[assembly: OwinStartup(typeof(CTROScheduleReport.Startup))]

namespace CTROScheduleReport
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "CTROScheduleService Configuring");
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=CTROReporting;User Id=panran;Password=Prss_1234;");
            //app.UseHangfireDashboard("/ctroreporting", new DashboardOptions
            //{
            //    Authorization = new[] { new MyAuthorizationFilter() }
            //});
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In case you need an OWIN context, use the next line, `OwinContext` class
            // is the part of the `Microsoft.Owin` package.
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return owinContext.Authentication.User.Identity.IsAuthenticated;
        }
    }
}
