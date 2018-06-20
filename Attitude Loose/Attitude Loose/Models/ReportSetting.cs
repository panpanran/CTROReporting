using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class ReportSetting
    {
        public int ReportSettingId { get; set; }

        public int ReportId { get; set; }

        public string Category { get; set; }

        public string Code { get; set; }

        public int Startrow { get; set; }

        public int Startcolumn { get; set; }

        public int AdditionStartrow { get; set; }

        public int AdditionStartcolumn { get; set; }

        public virtual Report Report { get; set; }

    }
}