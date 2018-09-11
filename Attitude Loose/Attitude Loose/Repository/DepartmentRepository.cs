using Attitude_Loose.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
{
    public class DepartmentRepository : RepositoryBase<Attitude_Loose.Models.Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IDepartmentRepository : IRepository<Attitude_Loose.Models.Department>
    {
    }
}