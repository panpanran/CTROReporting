namespace Attitude_Loose.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTopicMetric : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Topics", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.Topics", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Topics", new[] { "MetricId" });
            DropIndex("dbo.Topics", new[] { "UserId" });
            DropTable("dbo.Metrics");
            DropTable("dbo.Topics");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        TopicId = c.Int(nullable: false, identity: true),
                        TopicName = c.String(nullable: false, maxLength: 55),
                        Desc = c.String(maxLength: 100),
                        TopicType = c.Boolean(nullable: false),
                        MetricId = c.Int(),
                        UserId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TopicId);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        MetricId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.MetricId);
            
            CreateIndex("dbo.Topics", "UserId");
            CreateIndex("dbo.Topics", "MetricId");
            AddForeignKey("dbo.Topics", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Topics", "MetricId", "dbo.Metrics", "MetricId");
        }
    }
}
