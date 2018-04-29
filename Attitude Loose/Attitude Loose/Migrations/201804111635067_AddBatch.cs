namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBatch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MBatches",
                c => new
                    {
                        BatchId = c.Int(nullable: false, identity: true),
                        BatchName = c.String(),
                        BatchDesc = c.String(),
                        BatchType = c.Boolean(nullable: false),
                        FileId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BatchId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.MFiles",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.Int(nullable: false),
                        FilePath = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MBatches", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.MBatches", new[] { "User_Id" });
            DropTable("dbo.MFiles");
            DropTable("dbo.MBatches");
        }
    }
}
