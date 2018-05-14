using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public int DepartmentId { get; set; }

        public string ReportName { get; set; }
    }
}