using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
{
    public class ChartRepository : RepositoryBase<Chart>, IChartRepository
    {
        public ChartRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IChartRepository : IRepository<CTROLibrary.Model.Chart>
    {
    }
}