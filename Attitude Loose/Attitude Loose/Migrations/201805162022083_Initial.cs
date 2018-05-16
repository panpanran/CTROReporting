namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "StartDate", c => c.String());
            AddColumn("dbo.Records", "EndDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Records", "EndDate");
            DropColumn("dbo.Records", "StartDate");
        }
    }
}
