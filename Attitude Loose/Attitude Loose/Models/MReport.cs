using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class MReport
    {
        [Key]
        public int ReportId { get; set; }

        public int ReportName { get; set; }

        public int FileId { get; set; }

        public DateTime DateCreated { get; set; }

    }
}