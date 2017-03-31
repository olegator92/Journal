namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserInfo3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.UserInfoes");
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.AspNetUsers", new[] { "UserInfo_Id" });
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropColumn("dbo.AspNetUsers", "UserInfo_Id");
            DropTable("dbo.UserInfoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Name = c.String(),
                        Key = c.String(),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "UserInfo_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.UserInfoes", "WorkSchedule_Id");
            CreateIndex("dbo.AspNetUsers", "UserInfo_Id");
            AddForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            AddForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.UserInfoes", "Id", cascadeDelete: true);
        }
    }
}
