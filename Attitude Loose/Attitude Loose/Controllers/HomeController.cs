using Attitude_Loose.CTRO;
using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMetricService metricService;
        private IUserProfileService userProfileService;

        public HomeController(IMetricService metricService, IUserProfileService userProfileService)
        {
            this.metricService = metricService;
            this.userProfileService = userProfileService;
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
            var model = new ReportGenerateViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ReportGenerateViewModel model)
        {
            UserProfile userprofile = userProfileService.GetByUserID(User.Identity.GetUserId());
            model.ReportResult = false;

            if (ModelState.IsValid)
            {
                CTROHome home = new CTROHome();
                int turnround = 0;
                switch(model.SelectedReport)
                {
                    case "1":
                        turnround = await home.CreateTurnroundReportAsync(model.StartDate, model.EndDate, userprofile.Email);
                        break;
                    case "2":
                        turnround = await home.CreateSponsorNotMatcReportAsync(userprofile.Email);
                        break;
                }

                //int turnround = await home.CreateTurnroundReportAsync(model.StartDate, model.EndDate, userprofile.Email);
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