namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplate3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reports", "EmailId", "dbo.Emails");
            DropIndex("dbo.Reports", new[] { "EmailId" });
            AddColumn("dbo.Emails", "Name", c => c.String());
            AlterColumn("dbo.Reports", "EmailId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reports", "EmailId");
            AddForeignKey("dbo.Reports", "EmailId", "dbo.Emails", "EmailId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reports", "EmailId", "dbo.Emails");
            DropIndex("dbo.Reports", new[] { "EmailId" });
            AlterColumn("dbo.Reports", "EmailId", c => c.Int());
            DropColumn("dbo.Emails", "Name");
            CreateIndex("dbo.Reports", "EmailId");
            AddForeignKey("dbo.Reports", "EmailId", "dbo.Emails", "EmailId");
        }
    }
}
