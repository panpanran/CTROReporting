using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
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