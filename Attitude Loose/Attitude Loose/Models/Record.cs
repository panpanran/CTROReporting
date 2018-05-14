using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Record
    {
        public int RecordId { get; set; }

        public int ReportId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Record()
        {
            CreatedDate = DateTime.Now;
        }

    }
}