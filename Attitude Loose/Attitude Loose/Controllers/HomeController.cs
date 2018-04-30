using Attitude_Loose.CTRO;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMetricService metricService;
        public HomeController(IMetricService metricService)
        {
            this.metricService = metricService;
        }
        [HttpGet]
        public ActionResult Create()
        {
            var model = new TopicFormModel();
            var metrics = metricService.GetMetrics();
            //var goalstatus = goalStatusService.GetGoalStatus();
            model.Metrics = metricService.ToSelectListItems(metrics, -1);

            return PartialView(model);
        }

        [HttpGet]
        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Index()
        {
            //if (TempData["AlertMessage"] == null)
            //{
            //    TempData["AlertMessage"] = "";
            //}
            return View(new ReportGenerateViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(ReportGenerateViewModel model)
        {
            model.ReportResult = false;
            //if (TempData["AlertMessage"] == null)
            //{
            //    TempData["AlertMessage"] = "";
            //}
            if (ModelState.IsValid)
            {
                CTROHome home = new CTROHome();
                int turnround = await home.CreateTurnroundReportAsync(model.StartDate, model.EndDate);
                if (turnround == 1)
                {
                    model.ReportResult = true;
                }
            }

            return View(model);
        }

        [HandleError(View = "Error")]
        public ActionResult About()
        {
            int test = 0;
            int b = 1 / test;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}