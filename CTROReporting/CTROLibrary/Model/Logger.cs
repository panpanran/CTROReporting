using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace CTROLibrary.Model
{
    public class Logger
    {
        [Key]
        public int LogId { get; set; }

        public int Level { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public Logger()
        {
            CreatedDate = DateTime.Now;
        }
    }
}