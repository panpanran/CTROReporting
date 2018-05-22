using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
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