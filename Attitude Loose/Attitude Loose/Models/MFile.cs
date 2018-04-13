using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class MFile
    {
        [Key]
        public int FileId { get; set; }

        public int FileName { get; set; }

        public int FilePath { get; set; }

    }
}