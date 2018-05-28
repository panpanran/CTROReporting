using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public DateTime StartTime { get; set; }

        public int IntervalDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ReportId { get; set; }

        public virtual Report Report { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Schedule()
        {
            CreatedDate = DateTime.Now;
        }
    }
}