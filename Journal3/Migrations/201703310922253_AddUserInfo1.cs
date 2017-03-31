namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfo1 : DbMigration
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkSchedule_Id)
                .Index(t => t.WorkSchedule_Id);
            
            AddColumn("dbo.AspNetUsers", "UserInfo_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "UserInfo_Id");
            AddForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.UserInfoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.UserInfoes");
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "UserInfo_Id" });
            DropColumn("dbo.AspNetUsers", "UserInfo_Id");
            DropTable("dbo.UserInfoes");
        }
    }
}
