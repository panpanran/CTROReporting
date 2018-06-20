using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Service
{
    public interface IReportSettingService
    {
        IEnumerable<ReportSetting> GetReportSettingsByReportId(int reportid);
        ReportSetting GetReportSettingById(int reportsettingid);
    }

    public class ReportSettingService : IReportSettingService
    {
        private readonly IReportSettingRepository reportsettingRepository;
        private readonly IUnitOfWork unitOfWork;

        public ReportSettingService(IReportSettingRepository reportsettingRepository, IUnitOfWork unitOfWork)
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