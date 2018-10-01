namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "Active");
        }
    }
}
