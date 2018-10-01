namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "Template", c => c.String());
            AddColumn("dbo.Reports", "Savepath", c => c.String());
            AddColumn("dbo.ReportSettings", "Startrow", c => c.Int(nullable: false));
            AddColumn("dbo.ReportSettings", "Startcolumn", c => c.Int(nullable: false));
            AddColumn("dbo.ReportSettings", "AdditionStartrow", c => c.Int(nullable: false));
            AddColumn("dbo.ReportSettings", "AdditionStartcolumn", c => c.Int(nullable: false));
            DropColumn("dbo.ReportSettings", "Template");
            DropColumn("dbo.ReportSettings", "Savepath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportSettings", "Savepath", c => c.String());
            AddColumn("dbo.ReportSettings", "Template", c => c.String());
            DropColumn("dbo.ReportSettings", "AdditionStartcolumn");
            DropColumn("dbo.ReportSettings", "AdditionStartrow");
            DropColumn("dbo.ReportSettings", "Startcolumn");
            DropColumn("dbo.ReportSettings", "Startrow");
            DropColumn("dbo.Reports", "Savepath");
            DropColumn("dbo.Reports", "Template");
        }
    }
}
