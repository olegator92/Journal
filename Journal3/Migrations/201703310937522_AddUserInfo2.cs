namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfo2 : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkSchedule_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.WorkSchedule_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropTable("dbo.UserInfoes");
        }
    }
}
