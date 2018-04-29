using Attitude_Loose.App_Start;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Repository
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