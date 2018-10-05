using CTROLibrary;
using CTROReporting.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CTROReporting.CTRO
{
    public static class CTROHangfire
    {
        public static void Start()
        {
            List<Schedule> schedulelist = CTROLibrary.CTROFunctions.GetDataFromJson<List<Schedule>>("ScheduleService", "GetSchedules");
            ScheduleReports(schedulelist);
        }

        public static string GetJobName(Schedule schedule)
        {
            string jobname = schedule.ScheduleId.ToString() + " - " + schedule.User.UserName + " - " + schedule.Report.ReportName;
            return jobname;
        }

        public static void AddorUpdateJob(Schedule schedule)
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
            else
            {
                crontime = schedule.StartTime.Minute.ToString() + " " + schedule.StartTime.Hour.ToString() + " */" + schedule.IntervalDays + " * *";
            }
            RecurringJob.AddOrUpdate(GetJobName(schedule), () => ScheduledJob(schedule), crontime, TimeZoneInfo.Local);
        }

        public static void DeleteJob(Schedule schedule)
        {
            RecurringJob.RemoveIfExists(GetJobName(schedule));
        }

        public static void ScheduleReports(List<Schedule> schedulelist)
        {
            try
            {
                foreach (Schedule schedule in schedulelist)
                {
                    AddorUpdateJob(schedule);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROHangfire", MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public static void ScheduledJob(Schedule schedule)
        {
            int intervaldays = schedule.IntervalDays;
            int reportid = schedule.ReportId;
            string reportname = schedule.Report.ReportName;
            string userid = schedule.UserId;
            string startdate = DateTime.Now.AddDays(-intervaldays).ToString("yyyy-MM-dd");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                var report = CTROLibrary.CTROFunctions.GetDataFromJson<Report>("ReportService", "GetReportById", "reportid=" + reportid.ToString());
                var user = CTROLibrary.CTROFunctions.GetDataFromJson<ApplicationUser>("UserService", "GetByUserID", "userid=" + userid);

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
}