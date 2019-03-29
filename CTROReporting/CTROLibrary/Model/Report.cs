using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CTROLibrary.Model
{
    public class Report
    {
        public int ReportId { get; set; }

        // Foreign key
        public int DepartmentId { get; set; }

        public int EmailId { get; set; }

        public string ReportName { get; set; }

        public string Description { get; set; }

        public string Template { get; set; }

        public bool Active { get; set; }

        public string Savepath { get; set; }

        [JsonIgnore]
        public virtual Department Department { get; set; }

        public virtual Email Email { get; set; }

        [JsonIgnore]
        public virtual ICollection<Record> Records { get; set; }

        public virtual ICollection<ReportSetting> ReportSettings { get; set; }

        [JsonIgnore]
        public virtual ICollection<Schedule> Schedules { get; set; }

    }
}