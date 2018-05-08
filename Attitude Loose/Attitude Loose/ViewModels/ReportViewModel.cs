using Attitude_Loose.Test;
using System;
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

        public CTRPConst.ReportType ReportList { get; set; }

        public ReportGenerateViewModel()
        {
            ReportResult = false;
        }

    }

    public class ReportAnalysisViewModel
    {
        public string Xaxis { get; set; }
        public Dictionary<string, string> nYaxis { get; set; }
        public Dictionary<string, string> tYaxis { get; set; }
        public string[] Loginname { get; set; }
    }

    public class ReportProgressViewModel
    {
        public string ProgressPercentage { get; set; }
    }
}