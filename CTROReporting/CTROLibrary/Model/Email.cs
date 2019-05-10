using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTROLibrary.Model
{
    public class Email
    {
        [Key]
        public int EmailId { get; set; }

        public string Name { get; set; }
        [NotMapped]
        public string Subject { get; set; }
        [NotMapped]
        public string From { get; set; }
        [NotMapped]
        public string To { get; set; }
        public string Body { get; set; }
        [NotMapped]
        public string AttachmentFileName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Report> Reports { get; set; }
    }
}
