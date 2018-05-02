﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.ViewModels
{
    public class ReportViewModel
    {
    }

    public class ReportGenerateViewModel
    {
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public bool ReportResult { get; set; }

        [Required]
        [Display(Name = "Report Option")]
        public string SelectedReport { get; set; }

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