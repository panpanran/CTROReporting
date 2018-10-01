using CTROReporting.Infrastructure;
using CTROReporting.Models;
using CTROReporting.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CTROReporting.Service
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

    public class ScheduleServiceController : ApiController,IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IUnitOfWork unitOfWork;

        public ScheduleServiceController(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
        {
            this.scheduleRepository = scheduleRepository;
            this.unitOfWork = unitOfWork;
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
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

        public void CreateSchedule(Schedule schedule)
        {
            scheduleRepository.Add(schedule);
            SaveRecord();
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