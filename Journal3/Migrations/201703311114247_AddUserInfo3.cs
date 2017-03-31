namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfo3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        UserId = c.String(),
                        Name = c.String(),
                        Key = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkSchedules", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.WorkSchedules", "UserInfo_Id", c => c.Int());
            CreateIndex("dbo.WorkSchedules", "UserInfo_Id");
            AddForeignKey("dbo.WorkSchedules", "UserInfo_Id", "dbo.UserInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.WorkSchedules", "UserInfo_Id", "dbo.UserInfoes");
            DropIndex("dbo.WorkSchedules", new[] { "UserInfo_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropIndex("dbo.UserInfoes", new[] { "Id" });
            DropColumn("dbo.WorkSchedules", "UserInfo_Id");
            DropTable("dbo.UserInfoes");
        }
    }
}
