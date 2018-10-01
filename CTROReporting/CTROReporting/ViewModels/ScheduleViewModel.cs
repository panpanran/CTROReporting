using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTROReporting.ViewModels
{
    public class ScheduleViewModel
    {
    }

    public class ScheduleListViewModel
    {
        public int ScheduleId { get; set; }

        [Required]
        public string StartTime { get; set; }
        [Required]
        public int IntervalDays { get; set; }

        public string UserName { get; set; }
        [Required]
        public string ReportName { get; set; }

        public string CreatedDate { get; set; }
    }
}