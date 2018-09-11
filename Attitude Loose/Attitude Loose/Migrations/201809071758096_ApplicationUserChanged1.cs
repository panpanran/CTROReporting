namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUserChanged1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUsers", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUsers", "CreatedDate");
        }
    }
}
