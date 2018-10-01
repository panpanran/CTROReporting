using CTROReporting.App_Start;
using CTROReporting.Infrastructure;
using CTROReporting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
    }

    public class UserProfileRepository : RepositoryBase<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
        }
    }

}