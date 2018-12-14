using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CTROLibrary.Model
{
    public class Record
    {
        public int RecordId { get; set; }

        public int ReportId { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        [JsonIgnore]
        public virtual Report Report { get; set; }

        public Record()
        {
            CreatedDate = DateTime.Now;
        }

    }
}