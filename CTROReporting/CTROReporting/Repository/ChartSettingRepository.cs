using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
{
    public class ChartSettingRepository : RepositoryBase<ChartSetting>, IChartSettingRepository
    {
        public ChartSettingRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IChartSettingRepository : IRepository<ChartSetting>
    {
    }

}