using CTROReporting.Models;
using CTROReporting.Service;
using CTROReporting.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using AutoMapper;
using System.Linq;
using System.Web.Caching;
using CTROReporting.Infrastructure;

namespace CTROReporting.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private IUserProfileService userProfileService;
        private readonly IReportService reportService;
        private readonly IReportSettingService reportsettingService;
        private readonly IRecordService recordService;
        private IUserService userService;

        public ReportController(IUserService userService, IReportService reportService, IRecordService recordService, IUserProfileService userProfileService, IReportSettingService reportsettingService)
        {
            this.reportService = reportService;
            this.reportsettingService = reportsettingService;
            this.recordService = recordService;
            this.userProfileService = userProfileService;
            this.userService = userService;
        }

        public ActionResult GetReportList()
        {
            var reportlist = reportService.GetReports().Select(x => new { ReportName = x.ReportName }).OrderBy(x=>x.ReportName).ToList();
            return Json(reportlist, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ProgressBar(ReportProgressViewModel model)
        {
            //var createProgress = new ReportProgressViewModel();
            model.ProgressPercentage = "99";
            //for (double i = 1; i < 100; i++)
            //{
            //    string aa = ((i / 100) * 100).ToString();
            //    model.ProgressPercentage = aa;
            //}

            return PartialView(model);
        }

        [HttpGet]
        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Report()
        {
            ApplicationUser user = userService.GetByUserID(User.Identity.GetUserId());
            if (user != null)
            {
                user.LastLoginTime = DateTime.Now;
                userService.UpdateUser(user);
            }
            var model = new ReportGenerateViewModel();
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, -1);
            return View(model);
        }

        [HttpPost]
        public ActionResult Report(ReportGenerateViewModel model)
        {
            //Get Data from Cache
            UserProfile userprofile = userProfileService.GetByUserID(User.Identity.GetUserId());
            ApplicationUser user = userService.GetByUserID(User.Identity.GetUserId());

            model.ReportResult = false;

            if (ModelState.IsValid)
            {
                model.ReportResult = reportService.CreateReport(Convert.ToInt32(model.SelectedReport), User.Identity.GetUserId(),
                    model.StartDate, model.EndDate, user).Result;
            }
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, -1);
            return View(model);
        }

        [HttpGet]
        public ActionResult History()
        {
            var records = recordService.GetRecordsByUser(User.Identity.GetUserId()).ToList();

            var recordsList = Mapper.Map<IEnumerable<Record>, IEnumerable<RecordListViewModel>>(records).ToList();
            return View(recordsList);
        }
    }
}