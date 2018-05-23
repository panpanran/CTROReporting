using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Service
{
    public interface IScheduleService
    {
        void CreateRecord(Schedule schedule);
        void SaveRecord();
        IEnumerable<Schedule> GetSchedulesByUser(string userid);
        IEnumerable<Schedule> GetSchedules();
    }

    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IUnitOfWork unitOfWork;

        public ScheduleService(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            this.scheduleRepository = scheduleRepository;
            this.unitOfWork = unitOfWork;
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
        }

        public void CreateRecord(Schedule schedule)
        {
            scheduleRepository.Add(schedule);
            SaveRecord();
        }

        public IEnumerable<Schedule> GetSchedulesByUser(string userid)
        {
            var topic = scheduleRepository.GetMany(x => x.UserId == userid).OrderByDescending(g => g.CreatedDate);
            return topic;
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            var schedule = scheduleRepository.GetAll();
            return schedule;
        }
    }

}