using Attitude_Loose.CTRO;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using Attitude_Loose.Service;
using Autofac;
using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Test
{
    public class CTRPSchedule
    {
        public static void Start(List<Schedule> schedulelist)
        {
            RunProgram(schedulelist).GetAwaiter().GetResult();
        }

        private static async Task RunProgram(List<Schedule> schedulelist)
        {
            try
            {
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Clear();
                // and start it off
                await scheduler.Start();
                foreach (Schedule schedule in schedulelist)
                {
                     bool bb = await scheduler.CheckExists(new JobKey("job" + schedule.ScheduleId, "group" + schedule.ReportId));
                    List<IJobExecutionContext> jobs = scheduler.GetCurrentlyExecutingJobs().Result.ToList();
                    // define the job and tie it to our HelloJob class
                    IJobDetail job = JobBuilder.Create<ScheduleJob>()
                        .WithIdentity("job" + schedule.ScheduleId, "group" + schedule.ReportId)
                        .UsingJobData("starttime", DateTime.Now.ToString())
                        .UsingJobData("intervaldays", schedule.IntervalDays)
                        .UsingJobData("reportid", schedule.ReportId)
                        .UsingJobData("reportname", schedule.Report.ReportName)
                        .UsingJobData("userid", schedule.UserId)
                        .Build();

                    // Trigger the job to run now, and then repeat every 10 seconds
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
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }

    public class ScheduleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
           JobDataMap dataMap = context.JobDetail.JobDataMap;
            DateTime starttime = dataMap.GetDateTime("starttime");
            int intervaldays = dataMap.GetInt("intervaldays");
            int reportid = dataMap.GetInt("reportid");
            string reportname = dataMap.GetString("reportname").Replace(" - ", "");
            string userid = dataMap.GetString("userid");
            string startdate = starttime.AddDays(-intervaldays).ToString("yyyy-MM-dd");
            string enddate = starttime.ToString("yyyy-MM-dd");

            Record record = new Record
            {
                ReportId = reportid,
                UserId = userid,
                StartDate = startdate,
                EndDate = enddate,
            };
            DatabaseFactory factory = new DatabaseFactory();
            UnitOfWork unitOfWork = new UnitOfWork(factory);
            ReportSettingRepository reportSettingRepository = new ReportSettingRepository(factory);
            ReportRepository reportRepository = new ReportRepository(factory);
            Report report = reportRepository.GetById(reportid);
            UserRepository userRepository = new UserRepository(factory);
            ApplicationUser user = userRepository.Get(x=>x.Id == userid);

            CTROHome home = new CTROHome();
            string savepath = "";
            int result = home.CreateReport(startdate, enddate, user, report, out savepath);

            if (result == 1)
            {
                record.FilePath = "../Excel/" + user.UserName + "/" + Path.GetFileName(savepath);
                //Add Record
                RecordRepository recordRepository = new RecordRepository(factory);
                recordRepository.Add(record);
                unitOfWork.Commit();
            }
        }
    }
}