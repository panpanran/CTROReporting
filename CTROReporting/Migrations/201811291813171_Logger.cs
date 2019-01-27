namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Logger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loggers",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Level = c.String(),
                        ClassName = c.String(),
                        MethodName = c.String(),
                        Message = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loggers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Loggers", new[] { "UserId" });
            DropTable("dbo.Loggers");
        }
    }
}
