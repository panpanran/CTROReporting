using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.Now;
        }

        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool Activated { get; set; }

        public int RoleId { get; set; }

        public ICollection<Record> Records { get; set; }


        //public virtual ICollection<Goal> Goals { get; set; }

        //public virtual ICollection<FollowUser> FollowFromUser { get; set; }

        //public virtual ICollection<FollowUser> FollowToUser { get; set; }

        //public virtual ICollection<GroupRequest> GroupRequests { get; set; }

    }
}