namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeemail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Emails", "Subject", c => c.String());
            AddColumn("dbo.Emails", "From", c => c.String());
            AddColumn("dbo.Emails", "To", c => c.String());
            AddColumn("dbo.Emails", "Body", c => c.String());
            DropColumn("dbo.Emails", "Template");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Emails", "Template", c => c.String());
            DropColumn("dbo.Emails", "Body");
            DropColumn("dbo.Emails", "To");
            DropColumn("dbo.Emails", "From");
            DropColumn("dbo.Emails", "Subject");
        }
    }
}
