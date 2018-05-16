using Attitude_Loose.CTRO;
using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private IUserProfileService userProfileService;
        private readonly IReportService reportService;
        private readonly IRecordService recordService;
        public ReportController(IReportService reportService, IRecordService recordService, IUserProfileService userProfileService)
        {
            this.reportService = reportService;
            this.recordService = recordService;
            this.userProfileService = userProfileService;
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
        public ActionResult Analysis()
        {
            var model = new ReportAnalysisViewModel();
            model.AnalysisResult = false;

            CTROHome home = new CTROHome();
            string[] Xaxis = { };
            string[] ChartName = { };
            string[] ChartType = { };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };
            home.CreatPDAWorkloadAnalysisChart("2018-04-02", "2018-04-02", "", out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;

            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, "chart", -1);
            return View(model);
        }

        [HttpPost]
        public ActionResult Analysis(ReportAnalysisViewModel model)
        {
            CTROHome home = new CTROHome();
            string[] Xaxis = { };
            string[] ChartName = { };
            string[] ChartType = { };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };

            if (ModelState.IsValid)
            {
                if (model.SelectedAnalysis == "1")
                {
                    home.CreatPDAWorkloadAnalysisChart(model.StartDate, model.EndDate, "", out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                }
                model.AnalysisResult = true;
            }
            else
            {
                home.CreatPDAWorkloadAnalysisChart("2018-04-02", "2018-04-02", "", out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                model.AnalysisResult = false;
            }
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, "chart", -1);

            return View(model);
        }

        [HttpGet]
        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Report()
        {
            var model = new ReportGenerateViewModel();
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, "excel", -1);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Report(ReportGenerateViewModel model)
        {
            UserProfile userprofile = userProfileService.GetByUserID(User.Identity.GetUserId());
            model.ReportResult = false;
            //Add record to database
            Record record = new Record
            {
                ReportId = Convert.ToInt32(model.SelectedReport),
                UserId = User.Identity.GetUserId(),
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            if (ModelState.IsValid)
            {
                CTROHome home = new CTROHome();
                int turnround = 0;
                switch (model.SelectedReport)
                {
                    case "3":
                        turnround = await home.CreateTurnroundReportAsync(model.StartDate, model.EndDate, userprofile.Email);
                        break;
                    case "4":
                        turnround = await home.CreateSponsorNotMatcReportAsync(userprofile.Email);
                        break;
                }

                //int turnround = await home.CreateTurnroundReportAsync(model.StartDate, model.EndDate, userprofile.Email);
                if (turnround == 1)
                {
                    recordService.CreateRecord(record);
                    model.ReportResult = true;
                }
            }
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, "excel", -1);

            return View(model);
        }

    }
}