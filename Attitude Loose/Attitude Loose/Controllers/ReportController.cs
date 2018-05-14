﻿using Attitude_Loose.CTRO;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService reportService;
        private readonly ITopicService topicService;
        public ReportController(IReportService reportService, ITopicService topicService)
        {
            this.reportService = reportService;
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
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };
            home.CreatPDAWorkloadAnalysisChart("2018-04-02", "2018-04-02", "", out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;

            var reports = reportService.GetReports();
            model.Reports = reportService.ToSelectListItems(reports, -1);
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
                home.CreatPDAWorkloadAnalysisChart(model.StartDate, model.EndDate, "", out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
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
            model.Reports = reportService.ToSelectListItems(reports, -1);

            return View(model);
        }
    }
}