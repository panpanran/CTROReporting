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

    public class TurnroundViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
    }
}