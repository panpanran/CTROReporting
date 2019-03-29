using CTROLibrary.Model;
using CTROLibrary.Model.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CTROLibrary.DBbase
{
    public class EntitiesInitial : IdentityDbContext<ApplicationUser>
    {
        public EntitiesInitial()
            : base("CTROReportingEntities")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ReportSetting> ReportSetting { get; set; }
        public DbSet<Chart> Chart { get; set; }
        public DbSet<Logger> Logger { get; set; }
        public DbSet<ChartSetting> ChartSetting { get; set; }
        public DbSet<Email> Email { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer<EntitiesInitial>(null);
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
    //public class TopicSampleData : DropCreateDatabaseIfModelChanges<EntitiesInitial>
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