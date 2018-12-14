using CTROLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
{
    public class DepartmentRepository : RepositoryBase<CTROLibrary.Model.Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IDepartmentRepository : IRepository<CTROLibrary.Model.Department>
    {
    }
}