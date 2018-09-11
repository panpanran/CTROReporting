namespace CTRPReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportSettingChange : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ReportSettings", "ReportId");
            AddForeignKey("dbo.ReportSettings", "ReportId", "dbo.Reports", "ReportId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportSettings", "ReportId", "dbo.Reports");
            DropIndex("dbo.ReportSettings", new[] { "ReportId" });
        }
    }
}
