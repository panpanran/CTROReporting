using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.Models
{
    public class ChartSetting
    {
        public int ChartSettingId { get; set; }

        public int ChartId { get; set; }

        public string Code { get; set; }

        public string Category { get; set; }

        public string ChartType { get; set; }

        public string XLabel { get; set; }

        public string YLabel { get; set; }
        [JsonIgnore]
        public virtual Chart Chart { get; set; }
    }
}