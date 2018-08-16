using Attitude_Loose.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
{
    public class ChartRepository : RepositoryBase<Attitude_Loose.Models.Chart>, IChartRepository
    {
        public ChartRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IChartRepository : IRepository<Attitude_Loose.Models.Chart>
    {
    }

}