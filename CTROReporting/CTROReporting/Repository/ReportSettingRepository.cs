using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
{
    public class ReportSettingRepository : RepositoryBase<ReportSetting>, IReportSettingRepository
    {
        public ReportSettingRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IReportSettingRepository : IRepository<ReportSetting>
    {
    }
}