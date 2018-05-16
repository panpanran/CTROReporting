using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models.Configuration
{
    public class RecordConfiguration : EntityTypeConfiguration<Record>
    {
        public RecordConfiguration()
        {
            Property(u => u.RecordId).IsRequired();
            Property(u => u.ReportId).IsRequired();
            Property(u => u.CreatedDate).IsRequired();
            Property(u => u.StartDate).IsMaxLength();
            Property(u => u.EndDate).IsMaxLength();
            Property(u => u.UserId).HasMaxLength(50);
        }

    }
}