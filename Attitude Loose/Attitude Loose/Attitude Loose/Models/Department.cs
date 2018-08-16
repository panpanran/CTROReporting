﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }
        // Navigation property 
        [JsonIgnore]
        public virtual ICollection<Report> Reports { get; set; }
    }
}