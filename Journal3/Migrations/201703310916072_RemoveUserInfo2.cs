namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserInfo2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "UserId" });
            DropIndex("dbo.UserInfoes", new[] { "Id" });
            DropTable("dbo.UserInfoes");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.UserId);
            
            CreateIndex("dbo.UserInfoes", "Id");
            CreateIndex("dbo.UserInfoes", "UserId");
            AddForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
