using CTROReporting.Infrastructure;
using CTROReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Repository
{
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        public UserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IUserRepository : IRepository<ApplicationUser>
    {

    }
}