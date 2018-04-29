using Attitude_Loose.Models;
using Attitude_Loose.Models.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Attitude_Loose.App_Start
{
    public class EntitiesInitial : IdentityDbContext<ApplicationUser>
    {
        public EntitiesInitial()
            : base("AttitudeLooseEntities")
        {
        }
        public DbSet<Metric> Metrics { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<MBatch> Batches { get; set; }
        public DbSet<MFile> Files { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new UserProfileConfiguration());
            modelBuilder.Configurations.Add(new TopicConfiguration());
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }
    }

    public class TopicSampleData : CreateDatabaseIfNotExists<EntitiesInitial>
    {
        protected override void Seed(EntitiesInitial context)
        {
            new List<Metric>
            {
                new Metric { Type ="Entertainment"},
                new Metric { Type ="Politics"},
                new Metric { Type ="Life"}

            }.ForEach(m => context.Metrics.Add(m));

            context.Commit();

        }

    }
}