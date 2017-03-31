namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserInfoCorrection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "UserId" });
            DropIndex("dbo.UserInfoes", new[] { "Id" });
            DropPrimaryKey("dbo.UserInfoes");
            AddColumn("dbo.UserInfoes", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.UserInfoes", "WorkSchedule_Id", c => c.Int());
            AlterColumn("dbo.UserInfoes", "UserId", c => c.String());
            AlterColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserInfoes", "Id");
            CreateIndex("dbo.UserInfoes", "User_Id");
            CreateIndex("dbo.UserInfoes", "WorkSchedule_Id");
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropPrimaryKey("dbo.UserInfoes");
            AlterColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.UserInfoes", "UserId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.UserInfoes", "WorkSchedule_Id");
            DropColumn("dbo.UserInfoes", "User_Id");
            AddPrimaryKey("dbo.UserInfoes", "UserId");
            CreateIndex("dbo.UserInfoes", "Id");
            CreateIndex("dbo.UserInfoes", "UserId");
            AddForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
