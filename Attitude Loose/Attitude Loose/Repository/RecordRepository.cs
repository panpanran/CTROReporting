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

        public IEnumerable<Record> GetRecordsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy)
        {
            var skipRecords = noOfRecords * currentPage;

            var records = GetAll();

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
            records = (sortBy == "Date") ? records.OrderByDescending(g => g.CreatedDate) : records;
            records = (sortBy == "Popularity") ? records.OrderByDescending(g => g.ReportId) : records;

            records = records.Skip(skipRecords).Take(noOfRecords);

            return records.ToList();
        }
    }

    public interface IRecordRepository : IRepository<Record>
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
        IEnumerable<Record> GetRecordsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy);
    }

}