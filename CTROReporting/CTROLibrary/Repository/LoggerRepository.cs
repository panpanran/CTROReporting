using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
{
    public class LoggerRepository : RepositoryBase<Logger>, ILoggerRepository
    {
        public LoggerRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface ILoggerRepository : IRepository<Logger>
    {
    }

}