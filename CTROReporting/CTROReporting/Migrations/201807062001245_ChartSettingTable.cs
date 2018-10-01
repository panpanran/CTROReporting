namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChartSettingTable : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Charts", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.ChartSettings", "ChartId", "dbo.Charts");
            DropIndex("dbo.ChartSettings", new[] { "ChartId" });
            DropIndex("dbo.Charts", new[] { "DepartmentId" });
            DropTable("dbo.ChartSettings");
            DropTable("dbo.Charts");
        }
    }
}
