namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserInfo4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
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
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserInfoes", "User_Id");
            CreateIndex("dbo.UserInfoes", "WorkSchedule_Id");
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
        }
    }
}
