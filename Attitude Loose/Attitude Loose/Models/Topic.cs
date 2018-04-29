using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
{
    public class Topic
    {
        public int TopicId { get; set; }

        public string TopicName { set; get; }

        public string Desc { get; set; }

        public bool TopicType { get; set; }

        public int? MetricId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Metric Metric { get; set; }

        public Topic()
        {
            CreatedDate = DateTime.Now;
        }
    }
}