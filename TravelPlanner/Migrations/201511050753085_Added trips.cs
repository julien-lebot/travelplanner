namespace TravelPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedtrips : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Destination = c.String(),
                        Comment = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trips", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Trips", new[] { "UserId" });
            DropTable("dbo.Trips");
        }
    }
}
