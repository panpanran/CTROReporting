using CTROReporting.Infrastructure;
using CTROReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
{

    public class RecordRepository : RepositoryBase<Record>, IRecordRepository
    {
        public RecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }

    public interface IRecordRepository : IRepository<Record>
    {
    }

}