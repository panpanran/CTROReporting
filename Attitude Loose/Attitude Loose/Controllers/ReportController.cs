using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            //Thread.Sleep(1000);
            //string status = "Task Completed Successfully";
            //return Json(status, JsonRequestBehavior.AllowGet);
            var createProgress = new ReportProgressViewModel();
            createProgress.ProgressPercentage = "99%";
            return PartialView(createProgress);
        }
    }
}