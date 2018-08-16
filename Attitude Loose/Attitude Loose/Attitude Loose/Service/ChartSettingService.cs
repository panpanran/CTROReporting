using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Attitude_Loose.Service
{
    public interface IChartSettingService
    {
        IEnumerable<ChartSetting> GetChartSettingsByChartId(int Chartid);
        ChartSetting GetChartSettingById(int Chartsettingid);
    }

    public class ChartSettingServiceController : ApiController,IChartSettingService
    {
        private readonly IChartSettingRepository ChartsettingRepository;
        private readonly IUnitOfWork unitOfWork;

        public ChartSettingServiceController(IChartSettingRepository ChartsettingRepository, IUnitOfWork unitOfWork)
        {
            this.ChartsettingRepository = ChartsettingRepository;
            this.unitOfWork = unitOfWork;
        }

        public ChartSetting GetChartSettingById(int Chartsettingid)
        {
            var eportsetting = ChartsettingRepository.Get(x => x.ChartSettingId == Chartsettingid);
            return eportsetting;
        }

        public IEnumerable<ChartSetting> GetChartSettingsByChartId(int Chartid)
        {
            var ChartSettings = ChartsettingRepository.GetMany(x => x.ChartId == Chartid);
            return ChartSettings;
        }
    }

}