using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
{
    public class ScheduleRepository : RepositoryBase<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IScheduleRepository : IRepository<Schedule>
    {
    }
}