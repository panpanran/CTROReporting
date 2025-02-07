﻿using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace CTROReporting.Service
{
    public interface IRecordService
    {
        void CreateRecord(Record record);
        void SaveRecord();
        IEnumerable<Record> GetRecordsByUser(string userid);
    }

    [Authorize]
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

        public IHttpActionResult PostRecord(Record record)
        {
            recordRepository.Add(record);
            SaveRecord();
            return Ok();
        }

        public IEnumerable<Record> GetRecordsToday()
        {
            var records = recordRepository.GetAll().Where(x=>x.CreatedDate.Date == DateTime.Today.Date).OrderByDescending(x => x.CreatedDate);
            return records;
        }

        public IEnumerable<Record> GetRecordsByUser(string userid)
        {
            var record = recordRepository.GetMany(x => x.UserId == userid).OrderByDescending(g => g.CreatedDate);
            return record;
        }
    }
}