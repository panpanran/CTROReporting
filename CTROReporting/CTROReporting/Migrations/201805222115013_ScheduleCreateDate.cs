namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduleCreateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Schedules", "CreatedDate");
        }
    }
}
