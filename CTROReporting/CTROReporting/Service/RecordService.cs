using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using CTRPReporting.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace CTRPReporting.Service
{
    public interface IRecordService
    {
        void CreateRecord(Record record);
        void SaveRecord();
        IEnumerable<Record> GetRecordsByUser(string userid);
    }

    public class RecordServiceController : ApiController, IRecordService
    {
        private readonly IRecordRepository recordRepository;
        private readonly IUnitOfWork unitOfWork;

        public RecordServiceController(IRecordRepository recordRepository, IUnitOfWork unitOfWork)
        {
            this.recordRepository = recordRepository;
            this.unitOfWork = unitOfWork;
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
        }

        public void CreateRecord(Record record)
        {
            recordRepository.Add(record);
            SaveRecord();
        }


        public IEnumerable<Record> GetRecordsByUser(string userid)
        {
            var record = recordRepository.GetMany(x => x.UserId == userid).OrderByDescending(g => g.CreatedDate);
            return record;
        }
    }

}