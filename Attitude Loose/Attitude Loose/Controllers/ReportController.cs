using Attitude_Loose.CTRO;
using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using AutoMapper;
using System.Linq;
using System.IO;
using Attitude_Loose.Test;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private IUserProfileService userProfileService;
        private readonly IReportService reportService;
        private readonly IRecordService recordService;
        private readonly ITopicService topicService;

        public ReportController(IReportService reportService, IRecordService recordService, IUserProfileService userProfileService, ITopicService topicService)
        {
            this.reportService = reportService;
            this.recordService = recordService;
            this.userProfileService = userProfileService;
            this.topicService = topicService;
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
            string[] XLabel = { };
            string[] YLabel = { };

            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };

            home.CreateAnalysisChart("2018-04-02", "2018-04-02", "", "PDAAbstraction", out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;
            model.XLabel = XLabel;
            model.YLabel = YLabel;

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
            string[] XLabel = { };
            string[] YLabel = { };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };

            if (ModelState.IsValid)
            {
                string reportname = reportService.GetReportById(model.SelectedAnalysis).ReportName.Replace(" - ", "");
                home.CreateAnalysisChart(model.StartDate, model.EndDate, "", reportname, out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                model.AnalysisResult = true;
            }
            else
            {
                home.CreateAnalysisChart("2018-04-02", "2018-04-02", "", "PDAAbstraction", out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                model.AnalysisResult = false;
            }
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;
            model.XLabel = XLabel;
            model.YLabel = YLabel;
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
        public ActionResult Report(ReportGenerateViewModel model)
        {
            UserProfile userprofile = userProfileService.GetByUserID(User.Identity.GetUserId());
            model.ReportResult = false;
            //Add record to database
            Record record = new Record
            {
                ReportId = Convert.ToInt32(model.SelectedReport),
                UserId = User.Identity.GetUserId(),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
            };

            if (ModelState.IsValid)
            {
                string reportname = reportService.GetReportById(model.SelectedReport).ReportName;
                CTROHome home = new CTROHome();
                string savepath = "";
                int turnround = home.CreateReport(model.StartDate, model.EndDate, userprofile.Email, reportname, out savepath);

                if (turnround == 1)
                {
                    record.FilePath = "../Excel/" + Path.GetFileName(savepath);
                    recordService.CreateRecord(record);
                    model.ReportResult = true;
                }
            }
            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, "excel", -1);
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