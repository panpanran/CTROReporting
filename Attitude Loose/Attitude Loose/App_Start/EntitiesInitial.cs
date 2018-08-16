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
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ReportSetting> ReportSetting { get; set; }
        public DbSet<Attitude_Loose.Models.Chart> Chart { get; set; }
        public DbSet<ChartSetting> ChartSetting { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new UserProfileConfiguration());
            modelBuilder.Configurations.Add(new RecordConfiguration());
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
            new List<Department>
            {
                new Department { DepartmentName ="PDA"},
                new Department { DepartmentName ="SDA"},
                new Department { DepartmentName ="All"}

            }.ForEach(m => context.Departments.Add(m));


            context.Commit();

        }
    }
}