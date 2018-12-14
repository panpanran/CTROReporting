using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
{
    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IReportRepository : IRepository<Report>
    {
    }

}