using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Autofac;
using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static void Start(Schedule schedule)
        {
            RunProgram(schedule).GetAwaiter().GetResult();
        }

        private static async Task RunProgram(Schedule schedule)
        {
            try
            {
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    //.StartAt(DateBuilder.DateOf(16, 20, 0, 1, 5))
                    .StartAt(schedule.StartTime)
                    .WithSimpleSchedule(x => x
                        .WithInterval(new TimeSpan(schedule.IntervalDays,0,0,0))
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                //// some sleep to show what's happening
                //await Task.Delay(TimeSpan.FromSeconds(60));

                //// and last shut down the scheduler when you are ready to close your program
                //await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            CTRPFunctions.SendEmail("Ran Test", "XXXX", "ran.pan@nih.gov", @"C:\Users\panr2\Downloads\DataWarehouse\RanBackUp.txt");
        }
    }
}