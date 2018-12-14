using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
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