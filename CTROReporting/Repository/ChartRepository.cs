using CTRPReporting.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
{
    public class ChartRepository : RepositoryBase<CTRPReporting.Models.Chart>, IChartRepository
    {
        public ChartRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IChartRepository : IRepository<CTRPReporting.Models.Chart>
    {
    }

}