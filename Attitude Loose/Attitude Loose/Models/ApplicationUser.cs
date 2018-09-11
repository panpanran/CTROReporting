using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
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
        public DateTime CreatedDate { get; set; }

        public DateTime LastLoginTime { get; set; }

        public bool Activated { get; set; }

        // Foreign key
        public int? DepartmentId { get; set; }

        public int RoleId { get; set; }

        [JsonIgnore]
        public virtual Department Department { get; set; }

        [JsonIgnore]
        public virtual ICollection<Record> Records { get; set; }

        [JsonIgnore]
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}