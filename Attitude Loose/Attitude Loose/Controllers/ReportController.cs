using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

        public PartialViewResult ProgressBar()
        {
            //var createProgress = new ReportProgressViewModel();
            //model.ProgressPercentage = "0";
            //for (double i = 1; i < 100; i++)
            //{
            //    string aa = ((i / 100) * 100).ToString();
            //    model.ProgressPercentage = aa;
            //}

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult ProgressBar(ReportProgressViewModel model)
        {

            for (double i = 1; i < 10000000; i++)
            {
                string aa = ((i / 10000000) * 100).ToString();
                model.ProgressPercentage = aa;
            }
            //return new JsonResult { Data = "Successfully  uploaded" };

            return PartialView(model);
        }
    }
}