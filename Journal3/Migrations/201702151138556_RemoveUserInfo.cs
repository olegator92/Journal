namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserInfo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropTable("dbo.UserInfoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Key = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserInfoes", "WorkSchedule_Id");
            CreateIndex("dbo.UserInfoes", "User_Id");
            AddForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
