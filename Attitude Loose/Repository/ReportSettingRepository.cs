using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
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