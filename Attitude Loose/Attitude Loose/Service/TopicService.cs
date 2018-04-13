using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Service
{
    public interface ITopicService
    {
        void CreateTopic(Topic topic);
        void SaveTopic();
        IEnumerable<Topic> GetTopics();
        IEnumerable<Topic> GetTopicsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy);
    }

    public class TopicService:ITopicService
    {
        private readonly ITopicRepository topicRepository;
        private readonly IUnitOfWork unitOfWork;

        public TopicService(ITopicRepository topicRepository, IUnitOfWork unitOfWork)
        {
            this.topicRepository = topicRepository;
            this.unitOfWork = unitOfWork;
        }

        public void SaveTopic()
        {
            unitOfWork.Commit();
        }

        public void CreateTopic(Topic topic)
        {
            topicRepository.Add(topic);
            SaveTopic();
        }

        public IEnumerable<Topic> GetTopics()
        {
            var topic = topicRepository.GetMany(g => g.TopicType == false).OrderByDescending(g => g.CreatedDate);
            return topic;
        }

        public IEnumerable<Topic> GetTopicsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy)
        {
            return topicRepository.GetTopicsByPage(userId, currentPage, noOfRecords, sortBy, filterBy);
        }
        //public IEnumerable<ValidationResult> CanAddGoal(Topic newTopic, IUpdateService updateService)
        //{
        //    Goal goal;
        //    if (newGoal.GoalId == 0)
        //        goal = goalRepository.Get(g => g.GoalName == newGoal.GoalName);
        //    else
        //        goal = goalRepository.Get(g => g.GoalName == newGoal.GoalName && g.GoalId != newGoal.GoalId);
        //    if (goal != null)
        //    {
        //        yield return new ValidationResult("GoalName", Resources.GoalExists);
        //    }
        //    if (newGoal.StartDate.Subtract(newGoal.EndDate).TotalSeconds > 0)
        //    {
        //        yield return new ValidationResult("EndDate", Resources.EndDate);
        //    }

        //    int flag = 0;
        //    int status = 0;
        //    if (newGoal.GoalId != 0)
        //    {
        //        var Updates = updateService.GetUpdatesByGoal(newGoal.GoalId).OrderByDescending(g => g.UpdateDate).ToList();
        //        if (Updates.Count() > 0)
        //        {
        //            if ((Updates[0].UpdateDate.Subtract(newGoal.EndDate).TotalSeconds > 0))
        //            {
        //                flag = 1;
        //            }
        //            if ((newGoal.StartDate.Subtract(Updates[0].UpdateDate).TotalSeconds > 0))
        //            {
        //                status = 1;
        //            }
        //            if (flag == 1)
        //            {
        //                yield return new ValidationResult("EndDate", Resources.EndDateNotValid + " " + Updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
        //            }
        //            else if (status == 1)
        //            {
        //                yield return new ValidationResult("StartDate", Resources.StartDate + " " + Updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
        //            }
        //        }



        //    }
        //}
    }
}