using CTRPReporting.App_Start;
using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using CTRPReporting.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CTRPReporting.Service
{
    public interface IUserService
    {
        ApplicationUser GetByUserID(string userid);
        void UpdateUser(ApplicationUser user);
        IEnumerable<ApplicationUser> GetUsers();
        void DeleteUser(string userid);
        void CreateUser(ApplicationUser user);
        void SaveUser();
        void DisposeUser();
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

        public IEnumerable<ApplicationUser> GetUsers()
        {
            var users = userRepository.GetAll();
            return users;
        }

        public ApplicationUser GetByUserID(string userid)
        {
            var user = userRepository.Get(u => u.Id == userid);
            return user;
        }

        public void CreateUser(ApplicationUser user)
        {
            userRepository.Add(user);
            SaveUser();
        }

        public void UpdateUser(ApplicationUser user)
        {
            userRepository.Update(user);
            SaveUser();
        }

        public void DeleteUser(string userid)
        {
            var user = userRepository.Get(u => u.Id == userid);
            userRepository.Delete(user);
            SaveUser();
        }


        public void SaveUser()
        {
            unitOfWork.Commit();
        }

        public void DisposeUser()
        {
            unitOfWork.Dispose();
        }

    }
}