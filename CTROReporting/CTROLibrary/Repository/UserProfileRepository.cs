using CTROLibrary.DBbase;
using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CTROLibrary.Repository
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