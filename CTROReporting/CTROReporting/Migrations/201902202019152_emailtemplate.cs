namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "EmailId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "EmailId");
        }
    }
}
