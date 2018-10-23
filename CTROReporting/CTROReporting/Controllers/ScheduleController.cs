using CTROReporting.Models;
using CTROReporting.Service;
using CTROReporting.CTRO;
using CTROReporting.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quartz;

namespace CTROReporting.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService scheduleService;
        private readonly IReportService reportService;
        private readonly IUserService userService;

        public ScheduleController(IReportService reportService, IScheduleService scheduleService, IUserService userService)
        {
            this.reportService = reportService;
            this.scheduleService = scheduleService;
            this.userService = userService;
        }

        public ActionResult Schedule()
        {
            var schedules = scheduleService.GetSchedulesByUser(User.Identity.GetUserId()).ToList();

            var schedulesList = Mapper.Map<IEnumerable<Schedule>, IEnumerable<ScheduleListViewModel>>(schedules).ToList();
            return View(schedulesList);
        }

        public ActionResult CreateSchedule(ScheduleListViewModel model)
        {
            Schedule schedule = new Schedule();
            schedule.StartTime = Convert.ToDateTime(model.StartTime);
            schedule.IntervalDays = model.IntervalDays;
            schedule.ReportId = reportService.GetByReportName(model.ReportName).ReportId;
            schedule.UserId = User.Identity.GetUserId();
            schedule.Report = reportService.GetByReportName(model.ReportName);
            schedule.User = userService.GetByUserID(User.Identity.GetUserId());

            int toocloseschedules = scheduleService.GetSchedulesByUser(User.Identity.GetUserId()).Where(x => x.ReportId == schedule.ReportId && Math.Abs((x.StartTime.TimeOfDay-schedule.StartTime.TimeOfDay).TotalMinutes) < 5).Count();

            if (toocloseschedules > 0)
            {
                TempData["ScheduleResult"] = "false";
            }
            else
            {
                TempData["ScheduleResult"] = "true";
                CTROHangfire.AddorUpdateJob(schedule);
                scheduleService.CreateSchedule(schedule);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateSchedule(ScheduleListViewModel model)
        {
            Schedule schedule = scheduleService.GetByScheduleID(model.ScheduleId);
            schedule.StartTime = Convert.ToDateTime(model.StartTime);
            schedule.IntervalDays = model.IntervalDays;
            schedule.ReportId = reportService.GetByReportName(model.ReportName).ReportId;

            int toocloseschedules = scheduleService.GetSchedulesByUser(User.Identity.GetUserId()).Where(x => x.ReportId == schedule.ReportId && x.ScheduleId != schedule.ScheduleId && Math.Abs((x.StartTime.TimeOfDay - schedule.StartTime.TimeOfDay).TotalMinutes) < 5).Count();

            if (toocloseschedules > 0)
            {
                TempData["ScheduleResult"] = "false";
            }
            else
            {
                TempData["ScheduleResult"] = "true";
                CTROHangfire.AddorUpdateJob(schedule);
                scheduleService.UpdateSchedule(schedule);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSchedule(ScheduleListViewModel model)
        {
            var schedule = scheduleService.GetByScheduleID(model.ScheduleId);
            CTROHangfire.DeleteJob(schedule);
            scheduleService.DeleteSchedule(schedule);
            return View();
        }

    }
}