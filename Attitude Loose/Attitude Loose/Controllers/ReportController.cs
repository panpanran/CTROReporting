using Attitude_Loose.CTRO;
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
        private readonly IMetricService metricService;
        private readonly ITopicService topicService;
        public ReportController(IMetricService metricService, ITopicService topicService)
        {
            this.metricService = metricService;
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
            string Xaxis = "";
            Dictionary<string, string> nYaxis = new Dictionary<string, string>();
            Dictionary<string, string> tYaxis = new Dictionary<string, string>();
            string[] loginname = { };
            home.CreatAnalysisChart("2018-04-01", "2018-04-01", "", out Xaxis, out nYaxis, out tYaxis, out loginname);
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.nYaxis = nYaxis;
            model.tYaxis = tYaxis;
            return View(model);
        }

        [HttpPost]
        public ActionResult Analysis(ReportAnalysisViewModel model)
        {
            if (ModelState.IsValid)
            {
                CTROHome home = new CTROHome();
                string Xaxis = "";
                Dictionary<string, string> nYaxis = new Dictionary<string, string>();
                Dictionary<string, string> tYaxis = new Dictionary<string, string>();
                string[] loginname = { };
                home.CreatAnalysisChart(model.StartDate, model.EndDate, "", out Xaxis, out nYaxis, out tYaxis, out loginname);
                model.Loginname = loginname;
                model.Xaxis = Xaxis;
                model.nYaxis = nYaxis;
                model.tYaxis = tYaxis;
                model.AnalysisResult = true;
            }
            else
            {
                CTROHome home = new CTROHome();
                string Xaxis = "";
                Dictionary<string, string> nYaxis = new Dictionary<string, string>();
                Dictionary<string, string> tYaxis = new Dictionary<string, string>();
                string[] loginname = { };
                home.CreatAnalysisChart("2018-04-01", "2018-04-01", "", out Xaxis, out nYaxis, out tYaxis, out loginname);
                model.Loginname = loginname;
                model.Xaxis = Xaxis;
                model.nYaxis = nYaxis;
                model.tYaxis = tYaxis;
                model.AnalysisResult = false;
            }
            return View(model);
        }
    }
}