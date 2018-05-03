namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReport : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MBatches", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.MBatches", new[] { "User_Id" });
            DropTable("dbo.MBatches");
            DropTable("dbo.MFiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MFiles",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.Int(nullable: false),
                        FilePath = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileId);
            
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
                .PrimaryKey(t => t.BatchId);
            
            CreateIndex("dbo.MBatches", "User_Id");
            AddForeignKey("dbo.MBatches", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
