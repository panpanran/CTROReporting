namespace CTROReporting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class connectuserprofile1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.UserProfiles", name: "UserId", newName: "Id");
            AddColumn("dbo.UserProfiles", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserProfiles", "ApplicationUser_Id");
            AddForeignKey("dbo.UserProfiles", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserProfiles", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserProfiles", "ApplicationUser_Id");
            RenameColumn(table: "dbo.UserProfiles", name: "Id", newName: "UserId");
        }
    }
}
