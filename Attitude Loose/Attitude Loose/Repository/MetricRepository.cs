using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
{
    public class MetricRepository : RepositoryBase<Metric>, IMetricRepository
    {
        public MetricRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IMetricRepository : IRepository<Metric>
    {
    }
}