using CTROReporting.Infrastructure;
using CTROReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
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