using Attitude_Loose.App_Start;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Attitude_Loose.Service
{
    public interface IUserProfileService
    {
        void CreateUserProfile(string userId, string Email);
        UserProfile GetByUserID(string userid);
        void UpdateUserProfile(UserProfile user);
    }

    public class UserProfileServiceController : ApiController, IUserProfileService
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;


        public UserProfileServiceController(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
        }

        public void CreateUserProfile(string userId, string Email)
        {
            UserProfile newUserProfile = new UserProfile();
            newUserProfile.UserId = userId;
            newUserProfile.Email = Email;
            userProfileRepository.Add(newUserProfile);
            unitOfWork.Commit();
        }

        public UserProfile GetByUserID(string userid)
        {
            var userprofile = userProfileRepository.Get(u => u.UserId == userid);
            return userprofile;
        }

        public void UpdateUserProfile(UserProfile user)
        {
            userProfileRepository.Update(user);
            SaveUserProfile();
        }

        public void SaveUserProfile()
        {
            unitOfWork.Commit();
        }

    }
}