using CTROReporting.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
{
    public class ChartRepository : RepositoryBase<CTROReporting.Models.Chart>, IChartRepository
    {
        public ChartRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IChartRepository : IRepository<CTROReporting.Models.Chart>
    {
    }

}