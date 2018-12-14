using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using System.Collections.Generic;
using System.Web.Http;

namespace CTROReporting.Service
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