using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Reflection;
using System.Web.Caching;
using System.Runtime.Caching;
using CTROReporting.Infrastructure;
using CTROReporting.Repository;
using CTROReporting.Models;
using CTROReporting.EW;
using Hangfire;

namespace CTROReporting.CTRO
{
    public class CTROSchedule
    {
        public IScheduler Scheduler { get; set; }

        private object GetCache(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        public CTROSchedule()
        {
            //Cache
            string key = "ctroscheduler";
            Scheduler = (IScheduler)GetCache(key);
            if (Scheduler == null)
            {
                //initial scheduler
                IScheduler schedulertemp = StdSchedulerFactory.GetDefaultScheduler().Result;

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddDays(1))
                };
                MemoryCache.Default.Set(key, schedulertemp, cip);
                Scheduler = schedulertemp;
            }
        }

        public async Task Start(List<Schedule> schedulelist)
        {
            await ScheduleReports(schedulelist, Scheduler);
            //ScheduleFormatTickets().GetAwaiter().GetResult();
        }

        public async Task CreateJob(Schedule schedule)
        {
            if (Scheduler == null)
            {
                CTROSchedule ctroschedule = new CTROSchedule();
                DatabaseFactory factory = new DatabaseFactory();
                UnitOfWork unitOfWork = new UnitOfWork(factory);
                ScheduleRepository scheduleRepository = new ScheduleRepository(factory);
                List<Schedule> schedulelist = scheduleRepository.GetAll().ToList();
                await ctroschedule.Start(schedulelist);
            }

            try
            {
                IJobDetail job = JobBuilder.Create<ScheduleJob>()
                            .WithIdentity("job" + schedule.ScheduleId, "group" + schedule.ReportId)
                            .UsingJobData("starttime", DateTime.Now.ToString())
                            .UsingJobData("intervaldays", schedule.IntervalDays)
                            .UsingJobData("reportid", schedule.ReportId)
                            .UsingJobData("reportname", schedule.Report.ReportName)
                            .UsingJobData("userid", schedule.UserId)
                            .Build();

                // Trigger the job to run now, and then repeat every internaldays
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger" + schedule.ScheduleId, "group" + schedule.ReportId)
                    //.StartAt(DateBuilder.DateOf(16, 20, 0, 1, 5))
                    .StartAt(schedule.StartTime)
                    .WithSimpleSchedule(x => x
                        .WithInterval(new TimeSpan(schedule.IntervalDays, 0, 0, 0))
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await Scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROSchedule", MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public async Task UpdateJob(Schedule schedule)
        {
            try
            {
                if (DeleteJob(schedule))
                {
                   await CreateJob(schedule);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROSchedule", MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public bool DeleteJob(Schedule schedule)
        {
            JobKey jobkey = new JobKey("job" + schedule.ScheduleId, "group" + schedule.ReportId);
            try
            {
                if (Scheduler.CheckExists(jobkey).Result)
                {
                    return Scheduler.DeleteJob(jobkey).Result;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROSchedule", MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        private async Task ScheduleReports(List<Schedule> schedulelist, IScheduler scheduler)
        {
            try
            {
                await scheduler.Clear();
                // and start it off
                await scheduler.Start();
                foreach (Schedule schedule in schedulelist)
                {
                    // define the job and tie it to our HelloJob class
                    IJobDetail job = JobBuilder.Create<ScheduleJob>()
                        .WithIdentity("job" + schedule.ScheduleId, "group" + schedule.ReportId)
                        .UsingJobData("starttime", DateTime.Now.ToString())
                        .UsingJobData("intervaldays", schedule.IntervalDays)
                        .UsingJobData("reportid", schedule.ReportId)
                        .UsingJobData("reportname", schedule.Report.ReportName)
                        .UsingJobData("userid", schedule.UserId)
                        .Build();

                    // Trigger the job to run now, and then repeat every internaldays
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("trigger" + schedule.ScheduleId, "group" + schedule.ReportId)
                        //.StartAt(DateBuilder.DateOf(16, 20, 0, 1, 5))
                        .StartAt(schedule.StartTime)
                        .WithSimpleSchedule(x => x
                            .WithInterval(new TimeSpan(schedule.IntervalDays, 0, 0, 0))
                            .RepeatForever())
                        .Build();

                    // Tell quartz to schedule the job using our trigger
                    await scheduler.ScheduleJob(job, trigger);

                    //// some sleep to show what's happening
                    //await Task.Delay(TimeSpan.FromSeconds(60));

                    //// and last shut down the scheduler when you are ready to close your program
                    //await scheduler.Shutdown();
                }
            }
            catch (SchedulerException ex)
            {
                Logging.WriteLog("CTROSchedule", MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        //private static async Task ScheduleFormatTickets()
        //{
        //    try
        //    {
        //        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        //        await scheduler.Clear();
        //        //// and start it off
        //        await scheduler.Start();
        //        // define the job and tie it to our HelloJob class
        //        IJobDetail job = JobBuilder.Create<SimpleJob>()
        //                .WithIdentity("jobEW", "groupEW")
        //                .Build();

        //        // Trigger the job to run now, and then repeat every internaldays
        //        ITrigger trigger = TriggerBuilder.Create()
        //            .WithIdentity("triggerEW", "groupEW")
        //            //.StartAt(DateBuilder.DateOf(16, 20, 0, 1, 5))
        //            .StartAt(DateTime.Now)
        //            .WithSimpleSchedule(x => x
        //                .WithInterval(new TimeSpan(0, 5, 0))
        //                .RepeatForever())
        //            .Build();

        //        // Tell quartz to schedule the job using our trigger
        //        await scheduler.ScheduleJob(job, trigger);

        //    }
        //    catch (SchedulerException se)
        //    {
        //        Console.WriteLine(se);
        //    }
        //}

    }

    public class ScheduleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                DateTime starttime = dataMap.GetDateTime("starttime");
                int intervaldays = dataMap.GetInt("intervaldays");
                int reportid = dataMap.GetInt("reportid");
                string reportname = dataMap.GetString("reportname").Replace(" - ", "");
                string userid = dataMap.GetString("userid");
                string startdate = starttime.AddDays(-intervaldays).ToString("yyyy-MM-dd");
                string enddate = starttime.ToString("yyyy-MM-dd");

                var report = CTROLibrary.CTROFunctions.GetDataFromJson<Report>("ReportService", "GetReportById", "reportid="+ reportid.ToString());
                var user = CTROLibrary.CTROFunctions.GetDataFromJson<ApplicationUser>("UserService", "GetByUserID", "userid="+ userid);

                CTROHome home = new CTROHome();
                string savepath = "";
                int result = home.CreateReport(startdate, enddate, user, report, out savepath);
            }
            catch (Exception ex)
            {
                Logging.WriteLog("ScheduleJob", "Execute", ex.Message);
            }
        }
    }

    //public class SimpleJob : IJob
    //{
    //    public async Task Execute(IJobExecutionContext context)
    //    {
    //        EWFormatOriginalIncomingEmail ewFormat = new EWFormatOriginalIncomingEmail();
    //        string[] tickets = ewFormat.GetIDList("full_name is null and assigned_to_ is null").ToArray();
    //        ewFormat.BulkUpdate(tickets);
    //        EWTriageAccrual ewTriageAccrual = new EWTriageAccrual();
    //        ewTriageAccrual.BulkUpdate(tickets);
    //        EWTriageClinicalTrialsDotGov ewTriageClinicalTrialsDotGov = new EWTriageClinicalTrialsDotGov();
    //        ewTriageClinicalTrialsDotGov.BulkUpdate(tickets);
    //        EWTriageScientific ewEWTriageScientific = new EWTriageScientific();
    //        ewEWTriageScientific.BulkUpdate(tickets);
    //        EWTriageTSRFeedback ewTriageTSRFeedback = new EWTriageTSRFeedback();
    //        ewTriageTSRFeedback.BulkUpdate(tickets);
    //        EWTriageOnHoldTrials ewTriageOnHoldTrials = new EWTriageOnHoldTrials();
    //        ewTriageOnHoldTrials.BulkUpdate(tickets);
    //    }
    //}
}