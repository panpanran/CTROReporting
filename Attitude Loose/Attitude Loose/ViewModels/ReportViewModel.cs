using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attitude_Loose.ViewModels
{
    public class ReportViewModel
    {
    }

    public class ReportGenerateViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public bool ReportResult { get; set; }

        public ReportGenerateViewModel()
        {
            ReportResult = false;
        }

    }

    public class ReportProgressViewModel
    {
        public string ProgressPercentage { get; set; }
    }
}