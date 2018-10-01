using CTROReporting.CTRO;
using CTROReporting.Infrastructure;
using CTROReporting.Models;
using CTROReporting.Repository;
using CTROReporting.Service;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;

namespace CTROScheduleReport
{
    public partial class CTROScheduleService : ServiceBase
    {
        public CTROScheduleService()
        {
            InitializeComponent();
        }

        private Timer tickettimer = null;
        protected override void OnStart(string[] args)
        {
            tickettimer = new Timer();
            tickettimer.Interval = 120000; // every 3 minutes
            tickettimer.Elapsed += new System.Timers.ElapsedEventHandler(tickettimerTick);
            tickettimer.Enabled = true;
            Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "CTROScheduleService Start");
        }

        private void tickettimerTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                List<Schedule> schedulelist = CTROLibrary.CTROFunctions.GetDataFromJson<List<Schedule>>("ScheduleService", "GetSchedules");
                CTROSchedule ctroschedule = new CTROSchedule();
                ctroschedule.Start(schedulelist);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        protected override void OnStop()
        {
            if (StdSchedulerFactory.GetDefaultScheduler().Result.IsStarted)
            {
                StdSchedulerFactory.GetDefaultScheduler().Result.Shutdown();
            }
            Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "CTROScheduleService Stop");
        }
    }
}
