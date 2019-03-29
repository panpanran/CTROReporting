namespace CTROLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Charts",
                c => new
                    {
                        ChartId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        ChartName = c.String(),
                    })
                .PrimaryKey(t => t.ChartId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ChartSettings",
                c => new
                    {
                        ChartSettingId = c.Int(nullable: false, identity: true),
                        ChartId = c.Int(nullable: false),
                        Code = c.String(),
                        Category = c.String(),
                        ChartType = c.String(),
                        XLabel = c.String(),
                        YLabel = c.String(),
                    })
                .PrimaryKey(t => t.ChartSettingId)
                .ForeignKey("dbo.Charts", t => t.ChartId, cascadeDelete: true)
                .Index(t => t.ChartId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        LastLoginTime = c.DateTime(nullable: false),
                        Activated = c.Boolean(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Loggers",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        ClassName = c.String(),
                        MethodName = c.String(),
                        Message = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Records",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        ReportId = c.Int(nullable: false),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        FilePath = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Reports", t => t.ReportId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ReportId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        ReportId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        EmailId = c.Int(nullable: false),
                        ReportName = c.String(),
                        Description = c.String(),
                        Template = c.String(),
                        Active = c.Boolean(nullable: false),
                        Savepath = c.String(),
                    })
                .PrimaryKey(t => t.ReportId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Emails", t => t.EmailId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.EmailId);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        EmailId = c.Int(nullable: false, identity: true),
                        Template = c.String(),
                    })
                .PrimaryKey(t => t.EmailId);
            
            CreateTable(
                "dbo.ReportSettings",
                c => new
                    {
                        ReportSettingId = c.Int(nullable: false, identity: true),
                        ReportId = c.Int(nullable: false),
                        ReportType = c.String(),
                        Category = c.String(),
                        Code = c.String(),
                        Startrow = c.Int(nullable: false),
                        Startcolumn = c.Int(nullable: false),
                        AdditionStartrow = c.Int(nullable: false),
                        AdditionStartcolumn = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReportSettingId)
                .ForeignKey("dbo.Reports", t => t.ReportId, cascadeDelete: true)
                .Index(t => t.ReportId);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        ScheduleId = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        IntervalDays = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ReportId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dbo.Reports", t => t.ReportId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ReportId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        DateEdited = c.DateTime(nullable: false),
                        Email = c.String(),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(),
                        ProfilePicUrl = c.String(),
                        DateOfBirth = c.DateTime(),
                        Gender = c.Boolean(),
                        Address = c.String(),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        ZipCode = c.Double(),
                        ContactNo = c.Double(),
                        Id = c.String(maxLength: 50),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Charts", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Schedules", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Schedules", "ReportId", "dbo.Reports");
            DropForeignKey("dbo.ReportSettings", "ReportId", "dbo.Reports");
            DropForeignKey("dbo.Records", "ReportId", "dbo.Reports");
            DropForeignKey("dbo.Reports", "EmailId", "dbo.Emails");
            DropForeignKey("dbo.Reports", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Loggers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartSettings", "ChartId", "dbo.Charts");
            DropIndex("dbo.UserProfiles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Schedules", new[] { "UserId" });
            DropIndex("dbo.Schedules", new[] { "ReportId" });
            DropIndex("dbo.ReportSettings", new[] { "ReportId" });
            DropIndex("dbo.Reports", new[] { "EmailId" });
            DropIndex("dbo.Reports", new[] { "DepartmentId" });
            DropIndex("dbo.Records", new[] { "UserId" });
            DropIndex("dbo.Records", new[] { "ReportId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Loggers", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "DepartmentId" });
            DropIndex("dbo.ChartSettings", new[] { "ChartId" });
            DropIndex("dbo.Charts", new[] { "DepartmentId" });
            DropTable("dbo.UserProfiles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Schedules");
            DropTable("dbo.ReportSettings");
            DropTable("dbo.Emails");
            DropTable("dbo.Reports");
            DropTable("dbo.Records");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Loggers");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Departments");
            DropTable("dbo.ChartSettings");
            DropTable("dbo.Charts");
        }
    }
}
