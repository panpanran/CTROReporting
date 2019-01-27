namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loggerModify1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Loggers", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Loggers", "Level", c => c.String());
        }
    }
}
