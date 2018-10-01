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

        public ScheduleController(IReportService reportService, IScheduleService scheduleService)
        {
            this.reportService = reportService;
            this.scheduleService = scheduleService;
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
            scheduleService.CreateSchedule(schedule);

            CTROSchedule ctroSchedule = new CTROSchedule();
            ctroSchedule.CreateJob(schedule);

            return View();
        }


        public ActionResult UpdateSchedule(ScheduleListViewModel model)
        {
            Schedule schedule = scheduleService.GetByScheduleID(model.ScheduleId);
            schedule.StartTime = Convert.ToDateTime(model.StartTime);
            schedule.IntervalDays = model.IntervalDays;
            schedule.ReportId = reportService.GetByReportName(model.ReportName).ReportId;
            scheduleService.UpdateSchedule(schedule);

            CTROSchedule ctroSchedule = new CTROSchedule();
            ctroSchedule.UpdateJob(schedule);

            return View();
        }

        public ActionResult DeleteSchedule(ScheduleListViewModel model)
        {
            scheduleService.DeleteSchedule(model.ScheduleId);

            Schedule schedule = scheduleService.GetByScheduleID(model.ScheduleId);
            CTROSchedule ctroSchedule = new CTROSchedule();
            ctroSchedule.DeleteJob(schedule);

            return View();
        }

    }
}