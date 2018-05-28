using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Attitude_Loose.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
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

            //Refresh Schedules
            List<Schedule> schedulelist = scheduleService.GetSchedules().ToList();
            CTRPSchedule.Start(schedulelist);

            return View();
        }


        public ActionResult UpdateSchedule(ScheduleListViewModel model)
        {
            Schedule schedule = scheduleService.GetByScheduleID(model.ScheduleId);
            schedule.StartTime = Convert.ToDateTime(model.StartTime);
            schedule.IntervalDays = model.IntervalDays;
            schedule.ReportId = reportService.GetByReportName(model.ReportName).ReportId;
            scheduleService.UpdateSchedule(schedule);

            //Refresh Schedules
            List<Schedule> schedulelist = scheduleService.GetSchedules().ToList();
            CTRPSchedule.Start(schedulelist);

            return View();
        }

        public ActionResult DeleteSchedule(ScheduleListViewModel model)
        {
            scheduleService.DeleteSchedule(model.ScheduleId);

            //Refresh Schedules
            List<Schedule> schedulelist = scheduleService.GetSchedules().ToList();
            CTRPSchedule.Start(schedulelist);

            return View();
        }

    }
}