namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        EmailId = c.Int(nullable: false, identity: true),
                        Template = c.String(),
                    })
                .PrimaryKey(t => t.EmailId);
            
            AddColumn("dbo.Reports", "EmailId", c => c.Int(nullable: true));
            CreateIndex("dbo.Reports", "EmailId");
            AddForeignKey("dbo.Reports", "EmailId", "dbo.Emails", "EmailId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reports", "EmailId", "dbo.Emails");
            DropIndex("dbo.Reports", new[] { "EmailId" });
            DropColumn("dbo.Reports", "EmailId");
            DropTable("dbo.Emails");
        }
    }
}
