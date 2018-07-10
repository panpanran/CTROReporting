using System;
using System.Collections.Generic;
using System.Linq;
using Attitude_Loose.Models;
using System.Web;
using System.Web.Mvc;
using Attitude_Loose.Repository;
using Attitude_Loose.Infrastructure;

namespace Attitude_Loose.Service
{
    public interface IChartService
    {
        IEnumerable<Attitude_Loose.Models.Chart> GetCharts();
        Attitude_Loose.Models.Chart GetChartById(int Chartid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Attitude_Loose.Models.Chart> Charts, int selectedId);
        Attitude_Loose.Models.Chart GetByChartName(string name);
    }

    public class ChartService : IChartService
    {
        private readonly IChartRepository ChartRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRecordService recordService;
        private readonly IChartSettingService recportsettingService;

        public ChartService(IChartRepository ChartRepository, IUnitOfWork unitOfWork, IRecordService recordService, IChartSettingService recportsettingService)
        {
            this.recordService = recordService;
            this.recportsettingService = recportsettingService;
            this.ChartRepository = ChartRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Attitude_Loose.Models.Chart> GetCharts()
        {
            var Chart = ChartRepository.GetAll();
            return Chart;
        }

        public Attitude_Loose.Models.Chart GetChartById(int Chartid)
        {
            var Chart = ChartRepository.Get(x => x.ChartId == Chartid);
            return Chart;
        }

        public Attitude_Loose.Models.Chart GetByChartName(string name)
        {
            var schedule = ChartRepository.Get(x => x.ChartName == name);
            return schedule;
        }


        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Attitude_Loose.Models.Chart> Charts, int selectedId)
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