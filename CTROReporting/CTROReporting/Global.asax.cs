using CTROLibrary.DBbase;
using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using Hangfire;
using CTROLibrary.CTRO;
using CTROReporting.App_Start;
using System.Data.SqlClient;

namespace CTROReporting
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private BackgroundJobServer _backgroundJobServer;
        private string connString = ConfigurationManager.ConnectionStrings["CTROReportingEntities"].ConnectionString;
        protected void Application_Start()
        {
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(connString);

            _backgroundJobServer = new BackgroundJobServer();


            Database.SetInitializer(new TopicSampleData());
            IOCConfig.Run();
            AutoMapperConfig.Configure();

            //api
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
    .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            System.Web.Http.GlobalConfiguration.Configuration.Formatters
                .Remove(System.Web.Http.GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new CustomExceptionFilter());
            SqlDependency.Start(connString);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _backgroundJobServer.Dispose();
            SqlDependency.Stop(connString);
        }
    }
}
