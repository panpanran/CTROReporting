namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addreporttypechart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportSettings", "ReportType", c => c.String());
            DropColumn("dbo.Reports", "ReportType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reports", "ReportType", c => c.String());
            DropColumn("dbo.ReportSettings", "ReportType");
        }
    }
}
