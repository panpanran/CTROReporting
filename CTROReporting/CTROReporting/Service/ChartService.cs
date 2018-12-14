using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CTROLibrary.Repository;
using System.Web.Http;
using CTROLibrary.Infrastructure;

namespace CTROReporting.Service
{
    public interface IChartService
    {
        IEnumerable<CTROLibrary.Model.Chart> GetCharts();
        CTROLibrary.Model.Chart GetChartById(int Chartid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTROLibrary.Model.Chart> Charts, int selectedId);
        CTROLibrary.Model.Chart GetByChartName(string name);
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

        public IEnumerable<CTROLibrary.Model.Chart> GetCharts()
        {
            var chart = ChartRepository.GetAll();
            return chart;
        }

        public CTROLibrary.Model.Chart GetChartById(int Chartid)
        {
            var Chart = ChartRepository.Get(x => x.ChartId == Chartid);
            return Chart;
        }

        public CTROLibrary.Model.Chart GetByChartName(string name)
        {
            var schedule = ChartRepository.Get(x => x.ChartName == name);
            return schedule;
        }


        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<CTROLibrary.Model.Chart> Charts, int selectedId)
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