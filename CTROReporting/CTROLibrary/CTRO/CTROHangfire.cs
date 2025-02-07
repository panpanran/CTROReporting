﻿using CTROLibrary;
using CTROLibrary.EW;
using CTROLibrary.Model;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace CTROLibrary.CTRO
{
    public static class CTROHangfire
    {
        public static void Start()
        {
            List<Schedule> schedulelist = CTROFunctions.GetDataFromJson<List<Schedule>>("ScheduleService", "GetSchedules");
            ScheduleReports(schedulelist);
        }

        public static string GetJobName(Schedule schedule)
        {
            string jobname = schedule.ScheduleId.ToString() + " - " + schedule.User.UserName + " - " + schedule.Report.ReportName;
            return jobname;
        }

        public static void AddorUpdateJob(Schedule schedule)
        {
            try
            {
                string crontime = "";
                if (schedule.IntervalDays == 7)
                {
                    crontime = schedule.StartTime.Minute.ToString() + " " + schedule.StartTime.Hour.ToString() + " * * " + schedule.StartTime.DayOfWeek.ToString();
                }
                else if (schedule.IntervalDays == 1)
                {
                    crontime = schedule.StartTime.Minute.ToString() + " " + schedule.StartTime.Hour.ToString() + " * * 1-5";
                }
                else if (schedule.IntervalDays == 30 && schedule.StartTime.Day == 1)
                {
                    crontime = schedule.StartTime.Minute.ToString() + " " + schedule.StartTime.Hour.ToString() + " 1 * *";
                }
                else
                {
                    crontime = schedule.StartTime.Minute.ToString() + " " + schedule.StartTime.Hour.ToString() + " */" + schedule.IntervalDays + " * *";
                }
                RecurringJob.AddOrUpdate(GetJobName(schedule), () => ScheduledJob(schedule), crontime, TimeZoneInfo.Local);
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROHangfire", "AddorUpdateJob", ex);
                throw;
            }
        }

        public static void DeleteJob(Schedule schedule)
        {
            RecurringJob.RemoveIfExists(GetJobName(schedule));
        }


        public static void ScheduleReports(List<Schedule> schedulelist)
        {
            try
            {
                RecurringJob.AddOrUpdate("00 - panpanr - Ticket Triage", () => ScheduledTicket(), "*/15 * * * *", TimeZoneInfo.Local);

                foreach (Schedule schedule in schedulelist)
                {
                    AddorUpdateJob(schedule);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROHangfire", MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        //[AutomaticRetry(Attempts = 0)]
        private static readonly object balanceLock = new object();

        public static async Task ScheduledJob(Schedule schedule)
        {
            int intervaldays = schedule.IntervalDays;
            int reportid = schedule.ReportId;
            string reportname = schedule.Report.ReportName;
            string userid = schedule.UserId;
            string startdate = DateTime.Now.AddDays(-intervaldays).ToString("yyyy-MM-dd");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                var report = CTROFunctions.GetDataFromJson<Report>("ReportService", "GetReportById", "reportid=" + reportid.ToString());
                var user = CTROFunctions.GetDataFromJson<ApplicationUser>("UserService", "GetByUserID", "userid=" + userid);

                CTROHome home = new CTROHome();

                lock (balanceLock)
                {
                    string savepath = home.CreateReport(startdate, enddate, user, report).Result;

                    if (string.IsNullOrEmpty(savepath))
                    {
                        throw new InvalidOperationException("Create Report Failed !! Please check network connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("ScheduleJob", "Execute", ex);
                throw;
            }
        }

        public static async Task ScheduledTicket()
        {

            try
            {
                EWFormatOriginalIncomingEmail ewFormat = new EWFormatOriginalIncomingEmail();
                string[] tickets = ewFormat.GetIDList("full_name is null and assigned_to_ is null").ToArray();
                ewFormat.BulkUpdate(tickets);
                EWTriageAccrual ewTriageAccrual = new EWTriageAccrual();
                ewTriageAccrual.BulkUpdate(tickets);
                EWTriageClinicalTrialsDotGov ewTriageClinicalTrialsDotGov = new EWTriageClinicalTrialsDotGov();
                ewTriageClinicalTrialsDotGov.BulkUpdate(tickets);
                EWTriageScientific ewEWTriageScientific = new EWTriageScientific();
                ewEWTriageScientific.BulkUpdate(tickets);
                EWTriageTSRFeedback ewTriageTSRFeedback = new EWTriageTSRFeedback();
                ewTriageTSRFeedback.BulkUpdate(tickets);
                EWTriageOnHoldTrials ewTriageOnHoldTrials = new EWTriageOnHoldTrials();
                ewTriageOnHoldTrials.BulkUpdate(tickets);
                EWTriageSpam ewTriageSpam = new EWTriageSpam();
                ewTriageSpam.BulkUpdate(tickets);
                EWTriageAdministrative ewTriageAdministrative = new EWTriageAdministrative();
                ewTriageAdministrative.BulkUpdate(tickets);

            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROHangfire", MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }


    /// <summary>
    /// Attribute to skip a job execution if the same job is already running.
    /// Mostly taken from: http://discuss.hangfire.io/t/job-reentrancy-avoidance-proposal/607
    /// </summary>
    //public class SkipConcurrentExecutionAttribute : JobFilterAttribute, IServerFilter
    //{
    //    private readonly int _timeoutInSeconds;

    //    public SkipConcurrentExecutionAttribute(int timeoutInSeconds)
    //    {
    //        if (timeoutInSeconds < 0) throw new ArgumentException("Timeout argument value should be greater that zero.");

    //        _timeoutInSeconds = timeoutInSeconds;
    //    }


    //    public void OnPerforming(PerformingContext filterContext)
    //    {
    //        var resource = String.Format(
    //                             "{0}.{1}",
    //                            filterContext.Job.Type.FullName,
    //                            filterContext.Job.Method.Name);

    //        var timeout = TimeSpan.FromSeconds(_timeoutInSeconds);

    //        try
    //        {
    //            var distributedLock = filterContext.Connection.AcquireDistributedLock(resource, timeout);
    //            filterContext.Items["DistributedLock"] = distributedLock;
    //        }
    //        catch (Exception)
    //        {
    //            filterContext.Canceled = true;
    //            Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "");
    //        }
    //    }

    //    public void OnPerformed(PerformedContext filterContext)
    //    {
    //        if (!filterContext.Items.ContainsKey("DistributedLock"))
    //        {
    //            throw new InvalidOperationException("Can not release a distributed lock: it was not acquired.");
    //        }

    //        var distributedLock = (IDisposable)filterContext.Items["DistributedLock"];
    //        distributedLock.Dispose();
    //    }
    //}
}