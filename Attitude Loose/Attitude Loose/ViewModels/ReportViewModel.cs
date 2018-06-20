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

    public class ReporSettingViewModel
    {
        public int ReportSettingId { get; set; }

        public int ReportId { get; set; }

        public string Category { get; set; }

        public string Code { get; set; }

        public int Startrow { get; set; }

        public int Startcolumn { get; set; }

        public int AdditionStartrow { get; set; }

        public int AdditionStartcolumn { get; set; }

        public string ReportName { get; set; }

        public string Template { get; set; }

        public string Savepath { get; set; }
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

        public int ReportId { get; set; }

        public IEnumerable<SelectListItem> Reports { get; set; }

        public ReportGenerateViewModel()
        {
            ReportResult = false;
        }

    }

    public class ReportAnalysisViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Required]
        [Display(Name = "Analysis Option")]
        public string SelectedAnalysis { get; set; }

        public bool AnalysisResult { get; set; }

        public int? ReportId { get; set; }

        public IEnumerable<SelectListItem> Reports { get; set; }


        public string[] Xaxis { get; set; }
        public List<Dictionary<string, string>> Yaxis { get; set; }
        public string[] Loginname { get; set; }
        public string[] ChartName { get; set; }
        public string[] ChartType { get; set; }
        public string[] XLabel { get; set; }
        public string[] YLabel { get; set; }

        public ReportAnalysisViewModel()
        {
            Yaxis = new List<Dictionary<string, string>>();
            Loginname = new string[] {""};
        }
    }

    public class ReportProgressViewModel
    {
        public string ProgressPercentage { get; set; }
    }
}