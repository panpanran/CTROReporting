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

        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Index()
        {
            //if (TempData["AlertMessage"] == null)
            //{
            //    TempData["AlertMessage"] = "";
            //}
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(TurnroundViewModel model)
        {
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
                    TempData["AlertMessage"] = "Success";
                }
            }

            return View();
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