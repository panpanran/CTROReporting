using CTROReporting.Infrastructure;
using CTROReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
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