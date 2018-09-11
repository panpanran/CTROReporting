using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Repository
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