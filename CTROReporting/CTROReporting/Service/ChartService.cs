using System;
using System.Collections.Generic;
using System.Linq;
using CTROReporting.Models;
using System.Web;
using System.Web.Mvc;
using CTROReporting.Repository;
using CTROReporting.Infrastructure;
using System.Web.Http;

namespace CTROReporting.Service
{
    public interface IChartService
    {
        IEnumerable<CTROReporting.Models.Chart> GetCharts();
        CTROReporting.Models.Chart GetChartById(int Chartid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTROReporting.Models.Chart> Charts, int selectedId);
        CTROReporting.Models.Chart GetByChartName(string name);
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

        public IEnumerable<CTROReporting.Models.Chart> GetCharts()
        {
            var Chart = ChartRepository.GetAll();
            return Chart;
        }

        public CTROReporting.Models.Chart GetChartById(int Chartid)
        {
            var Chart = ChartRepository.Get(x => x.ChartId == Chartid);
            return Chart;
        }

        public CTROReporting.Models.Chart GetByChartName(string name)
        {
            var schedule = ChartRepository.Get(x => x.ChartName == name);
            return schedule;
        }


        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTROReporting.Models.Chart> Charts, int selectedId)
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