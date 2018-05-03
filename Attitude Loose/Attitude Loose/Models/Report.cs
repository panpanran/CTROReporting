using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public string ReportType { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Report()
        {
            CreatedDate = DateTime.Now;
        }

    }
}