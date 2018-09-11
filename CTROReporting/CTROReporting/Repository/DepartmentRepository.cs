using CTRPReporting.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
{
    public class DepartmentRepository : RepositoryBase<CTRPReporting.Models.Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IDepartmentRepository : IRepository<CTRPReporting.Models.Department>
    {
    }
}