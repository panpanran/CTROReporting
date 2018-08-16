using Attitude_Loose.App_Start;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Attitude_Loose.Service
{
    public interface IUserService
    {
        ApplicationUser GetByUserID(string userid);
        void UpdateUser(ApplicationUser user);
    }

    public class UserServiceController : ApiController,IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;


        public UserServiceController(IUserRepository userRepository, IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
        }

        public ApplicationUser GetByUserID(string userid)
        {
            var userprofile = userRepository.Get(u => u.Id == userid);
            return userprofile;
        }

        public void UpdateUser(ApplicationUser user)
        {
            userRepository.Update(user);
            SaveUser();
        }

        public void SaveUser()
        {
            unitOfWork.Commit();
        }
    }
}