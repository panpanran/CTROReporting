using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models.Configuration
{
    public class TopicConfiguration : EntityTypeConfiguration<Topic>
    {
        public TopicConfiguration()
        {
            Property(g => g.TopicName).IsRequired().HasMaxLength(55);
            Property(g => g.TopicType).IsRequired();
            Property(g => g.CreatedDate).IsRequired();
            Property(g => g.Desc).HasMaxLength(100);
        }
    }
}