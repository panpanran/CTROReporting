using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.ViewModels
{
    public class ChartViewModel
    {
    }

    public class ChartAnalysisViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Required]
        [Display(Name = "Chart Option")]
        public string SelectedChart { get; set; }

        public bool AnalysisResult { get; set; }

        public int? ChartId { get; set; }

        public IEnumerable<SelectListItem> Charts { get; set; }


        public string[] Xaxis { get; set; }
        public List<Dictionary<string, string>> Yaxis { get; set; }
        public string[] Loginname { get; set; }
        public string[] ChartName { get; set; }
        public string[] ChartType { get; set; }
        public string[] XLabel { get; set; }
        public string[] YLabel { get; set; }

        public ChartAnalysisViewModel()
        {
            Yaxis = new List<Dictionary<string, string>>();
            Loginname = new string[] { "" };
        }
    }
}