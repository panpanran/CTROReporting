using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Batch
    {
        public int BatchId { get; set; }

        public string BatchName { set; get; }

        public string BatchDesc { get; set; }

        public bool BatchType { get; set; }

        public int? FileId { get; set; }

        public string UserId { get; set; }
    }
}