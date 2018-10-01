using CTROReporting.Infrastructure;
using CTROReporting.Models;
using CTROReporting.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CTROReporting.Service
{
    public interface IReportSettingService
    {
        IEnumerable<ReportSetting> GetReportSettingsByReportId(int reportid);
        ReportSetting GetReportSettingById(int reportsettingid);
    }

    public class ReportSettingServiceController : ApiController,IReportSettingService
    {
        private readonly IReportSettingRepository reportsettingRepository;
        private readonly IUnitOfWork unitOfWork;

        public ReportSettingServiceController(IReportSettingRepository reportsettingRepository, IUnitOfWork unitOfWork)
        {
            this.reportsettingRepository = reportsettingRepository;
            this.unitOfWork = unitOfWork;
        }

        public ReportSetting GetReportSettingById(int reportsettingid)
        {
            var eportsetting = reportsettingRepository.Get(x => x.ReportSettingId == reportsettingid);
            return eportsetting;
        }

        public IEnumerable<ReportSetting> GetReportSettingsByReportId(int reportid)
        {
            var ReportSettings = reportsettingRepository.GetMany(x => x.ReportId == reportid);
            return ReportSettings;
        }
    }
}