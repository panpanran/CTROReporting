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

        public ActionResult Analysis(ReportAnalysisViewModel model)
        {
            CTROHome home = new CTROHome();
            string Xaxis = "";
            Dictionary<string, string> Yaxis = new Dictionary<string, string>();
            string[] loginname = { };
            home.CreatAnalysisChart("2018-04-01", "2018-04-30", "", out Xaxis, out Yaxis, out loginname);
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;

            //List<string> list = new List<string>();
            //list.Add("Week1");
            //list.Add("Week2");
            //list.Add("Week3");
            //list.Add("Week4");

            //model.Xaxis = list.ToArray() ;
            //model.Yaxis = "1, 2, 3, 1";
            return View(model);
        }
    }
}