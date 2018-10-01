using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        // Navigation property 
        [JsonIgnore]
        public virtual ICollection<Report> Reports { get; set; }

        //[JsonIgnore]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}