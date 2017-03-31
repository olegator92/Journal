namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Key = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.WorkSchedules", t => t.Id, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserInfoes", new[] { "Id" });
            DropIndex("dbo.UserInfoes", new[] { "UserId" });
            DropTable("dbo.UserInfoes");
        }
    }
}
