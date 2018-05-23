using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.ViewModels
{
    public class ScheduleViewModel
    {
    }

    public class ScheduleListViewModel
    {
        public int ScheduleId { get; set; }

        public string StartTime { get; set; }

        public int IntervalDays { get; set; }

        public string UserName { get; set; }

        public string ReportName { get; set; }

        public string CreatedDate { get; set; }
    }
}