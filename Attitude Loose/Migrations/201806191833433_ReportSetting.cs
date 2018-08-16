namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportSettings",
                c => new
                    {
                        ReportSettingId = c.Int(nullable: false, identity: true),
                        ReportId = c.Int(nullable: false),
                        Code = c.String(),
                        Template = c.String(),
                        Savepath = c.String(),
                    })
                .PrimaryKey(t => t.ReportSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReportSettings");
        }
    }
}
