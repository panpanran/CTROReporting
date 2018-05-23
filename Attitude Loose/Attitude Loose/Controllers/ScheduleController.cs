using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
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

        public ScheduleController(IScheduleService scheduleService)
        {
            this.scheduleService = scheduleService;
        }

        public ActionResult Schedule(ScheduleListViewModel model)
        {
            var schedules = scheduleService.GetSchedulesByUser(User.Identity.GetUserId()).ToList();

            var schedulesList = Mapper.Map<IEnumerable<Schedule>, IEnumerable<ScheduleListViewModel>>(schedules).ToList();
            return View(schedulesList);
        }

    }
}