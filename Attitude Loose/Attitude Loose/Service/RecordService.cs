using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Service
{
    public interface IRecordService
    {
        void CreateRecord(Record record);
        void SaveRecord();
        IEnumerable<Record> GetRecords();
        IEnumerable<Record> GetRecordsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy);
    }

    public class RecordService : IRecordService
    {
        private readonly IRecordRepository recordRepository;
        private readonly IUnitOfWork unitOfWork;

        public RecordService(IRecordRepository recordRepository, IUnitOfWork unitOfWork)
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

        public IEnumerable<Record> GetRecords()
        {
            var topic = recordRepository.GetAll().OrderByDescending(g => g.CreatedDate);
            return topic;
        }

        public IEnumerable<Record> GetRecordsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy)
        {
            return recordRepository.GetRecordsByPage(userId, currentPage, noOfRecords, sortBy, filterBy);
        }
    }

}