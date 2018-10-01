namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportSettingCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportSettings", "Category", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportSettings", "Category");
        }
    }
}
