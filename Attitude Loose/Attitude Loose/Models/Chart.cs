using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Chart
    {
        public int ChartId { get; set; }

        // Foreign key
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public string ChartName { get; set; }

        public virtual ICollection<ChartSetting> ChartSettings { get; set; }

    }
}