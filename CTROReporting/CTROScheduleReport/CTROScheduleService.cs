using CTROLibrary;
using CTROLibrary.CTRO;
using Hangfire;
using Microsoft.Owin.Hosting;
using System;
using System.Reflection;
using System.ServiceProcess;

namespace CTROScheduleReport
{
    public partial class CTROScheduleService : ServiceBase
    {
        private BackgroundJobServer _server;
        public CTROScheduleService()
        {
            InitializeComponent();
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=CTROReporting;User Id=panran;Password=Prss_1234;");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add("http://localhost:9095");
                WebApp.Start<Startup>(options);
                CTROHangfire.Start();
                _server = new BackgroundJobServer();
            }
            catch (Exception ex)
            {
                //Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            //Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "CTROScheduleService Start");
        }

        protected override void OnStop()
        {
            _server.Dispose();
            //Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "CTROScheduleService Stop");
        }
    }
}
