using Attitude_Loose.CTRO;
using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        private readonly IChartService chartService;
        private readonly IChartSettingService chartsettingService;

        public ChartController(IChartService chartService, IChartSettingService chartsettingService)
        {
            this.chartService = chartService;
            this.chartsettingService = chartsettingService;
        }

        [HttpGet]
        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Chart()
        {
            var model = new ChartAnalysisViewModel();
            model.AnalysisResult = false;

            CTROHome home = new CTROHome();
            string[] Xaxis = { };
            string[] ChartName = { };
            string[] ChartType = { };
            string[] XLabel = { };
            string[] YLabel = { };

            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };
            Attitude_Loose.Models.Chart chart = chartService.GetChartById(1);

            home.CreateChart("2018-04-02", "2018-04-02", chart, out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;
            model.XLabel = XLabel;
            model.YLabel = YLabel;

            var reports = chartService.GetCharts();
            model.Charts = chartService.ToSelectListItems(reports, -1);
            return View(model);
        }

        [HttpPost]
        public ActionResult Chart(ChartAnalysisViewModel model)
        {
            CTROHome home = new CTROHome();

            string[] Xaxis = { };
            string[] ChartName = { };
            string[] ChartType = { };
            string[] XLabel = { };
            string[] YLabel = { };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            string[] loginname = { };

            Attitude_Loose.Models.Chart chart = chartService.GetChartById(Convert.ToInt32(model.SelectedChart));

            if (ModelState.IsValid)
            {
                home.CreateChart(model.StartDate, model.EndDate, chart, out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                model.AnalysisResult = true;
            }
            else
            {
                chart = chartService.GetChartById(1);
                home.CreateChart("2018-04-02", "2018-04-02", chart, out XLabel, out YLabel, out Xaxis, out ChartName, out ChartType, out Yaxis, out loginname);
                model.AnalysisResult = false;
            }
            model.Loginname = loginname;
            model.Xaxis = Xaxis;
            model.Yaxis = Yaxis;
            model.ChartName = ChartName;
            model.ChartType = ChartType;
            model.XLabel = XLabel;
            model.YLabel = YLabel;
            var reports = chartService.GetCharts();
            model.Charts = chartService.ToSelectListItems(reports, -1);

            return View(model);
        }
    }
}