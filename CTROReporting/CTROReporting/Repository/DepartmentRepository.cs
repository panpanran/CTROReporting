using CTROReporting.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
{
    public class DepartmentRepository : RepositoryBase<CTROReporting.Models.Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IDepartmentRepository : IRepository<CTROReporting.Models.Department>
    {
    }
}