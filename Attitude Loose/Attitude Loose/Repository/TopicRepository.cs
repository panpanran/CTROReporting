using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
{
    public class TopicRepository : RepositoryBase<Topic>, ITopicRepository
    {
        public TopicRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IEnumerable<Topic> GetTopicsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy)
        {
            var skipTopics = noOfRecords * currentPage;

            var topics = this.GetMany(g => g.TopicType == false);

            //for filter options
            //Following topics
            //if (filterBy == "My Followings topics")
            //{
            //    topics = from g in topics
            //            where (from f in this.DataContext.FollowUser.Where(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(g.UserId)
            //            select g;
            //}
            ////User topics
            //else if (filterBy == "My topics")
            //{
            //    topics = topics.Where(g => g.UserId == userId);
            //}
            ////Followed topics
            //else if (filterBy == "My Followed topics")
            //{
            //    topics = from g in topics
            //            join s in this.DataContext.Support on g.GoalId equals s.GoalId
            //            where s.UserId == userId
            //            select g;
            //}

            //for sorting based on date and popularity
            topics = (sortBy == "Date") ? topics.OrderByDescending(g => g.CreatedDate) : topics;
            //topics = (sortBy == "Popularity") ? topics.OrderByDescending(g => g.Supports.Count()) : topics;

            topics = topics.Skip(skipTopics).Take(noOfRecords);

            return topics.ToList();
        }
    }

    public interface ITopicRepository : IRepository<Topic>
    {
        /// <summary>
        /// /// Method will return topics as different page with specified number of records ,filter condition and sort criteria
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noOfRecords"></param>
        /// <param name="sortBy"></param>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        IEnumerable<Topic> GetTopicsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy);
    }
}
