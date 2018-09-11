using CTRPReporting.App_Start;
using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
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