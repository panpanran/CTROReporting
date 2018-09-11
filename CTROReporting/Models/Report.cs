using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTRPReporting.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        // Foreign key
        public int DepartmentId { get; set; }

        public string ReportName { get; set; }

        public string Template { get; set; }

        public bool Active { get; set; }

        public string Savepath { get; set; }

        [JsonIgnore]
        public virtual Department Department { get; set; }

        [JsonIgnore]
        public virtual ICollection<Record> Records { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReportSetting> ReportSettings { get; set; }

        [JsonIgnore]
        public virtual ICollection<Schedule> Schedules { get; set; }

    }
}