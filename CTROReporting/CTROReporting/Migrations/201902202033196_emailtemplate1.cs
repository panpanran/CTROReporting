namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplate1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reports", "EmailId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reports", "EmailId", c => c.Int(nullable: false));
        }
    }
}
