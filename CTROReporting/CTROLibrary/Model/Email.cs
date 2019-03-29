using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Template { get; set; }

        [JsonIgnore]
        public virtual ICollection<Report> Reports { get; set; }
    }
}
