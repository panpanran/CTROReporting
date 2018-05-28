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
        void CreateSchedule(Schedule schedule);
        void SaveSchedule();
        Schedule GetByScheduleID(int scheduleid);
        IEnumerable<Schedule> GetSchedulesByUser(string userid);
        IEnumerable<Schedule> GetSchedules();
        void UpdateSchedule(Schedule schedule);
        void DeleteSchedule(int id);
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

        public void CreateSchedule(Schedule schedule)
        {
            scheduleRepository.Add(schedule);
            SaveRecord();
        }

        public Schedule GetByScheduleID(int scheduleid)
        {
            var schedule = scheduleRepository.GetById(scheduleid);
            return schedule;
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

        public void UpdateSchedule(Schedule schedule)
        {
            scheduleRepository.Update(schedule);
            SaveSchedule();
        }

        public void DeleteSchedule(int id)
        {
            var schedule = scheduleRepository.GetById(id);
            scheduleRepository.Delete(schedule);
            SaveSchedule();
        }

        public void SaveSchedule()
        {
            unitOfWork.Commit();
        }

    }

}