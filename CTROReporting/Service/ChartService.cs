using System;
using System.Collections.Generic;
using System.Linq;
using CTRPReporting.Models;
using System.Web;
using System.Web.Mvc;
using CTRPReporting.Repository;
using CTRPReporting.Infrastructure;
using System.Web.Http;

namespace CTRPReporting.Service
{
    public interface IChartService
    {
        IEnumerable<CTRPReporting.Models.Chart> GetCharts();
        CTRPReporting.Models.Chart GetChartById(int Chartid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTRPReporting.Models.Chart> Charts, int selectedId);
        CTRPReporting.Models.Chart GetByChartName(string name);
    }

    public class ChartServiceController : ApiController,IChartService
    {
        private readonly IChartRepository ChartRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRecordService recordService;
        private readonly IChartSettingService recportsettingService;

        public ChartServiceController(IChartRepository ChartRepository, IUnitOfWork unitOfWork, IRecordService recordService, IChartSettingService recportsettingService)
        {
            this.recordService = recordService;
            this.recportsettingService = recportsettingService;
            this.ChartRepository = ChartRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<CTRPReporting.Models.Chart> GetCharts()
        {
            var Chart = ChartRepository.GetAll();
            return Chart;
        }

        public CTRPReporting.Models.Chart GetChartById(int Chartid)
        {
            var Chart = ChartRepository.Get(x => x.ChartId == Chartid);
            return Chart;
        }

        public CTRPReporting.Models.Chart GetByChartName(string name)
        {
            var schedule = ChartRepository.Get(x => x.ChartName == name);
            return schedule;
        }


        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTRPReporting.Models.Chart> Charts, int selectedId)
        {
            return
                Charts.OrderBy(Chart => Chart.ChartName)
                      .Select(Chart =>
                          new SelectListItem
                          {
                              Selected = (Chart.ChartId == selectedId),
                              Text = Chart.ChartName,
                              Value = Chart.ChartId.ToString()
                          });
        }
    }
}