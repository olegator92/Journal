namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkScheduleDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkSchedules", "UserInfo_Id", "dbo.UserInfoes");
            DropForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "Id" });
            DropIndex("dbo.WorkSchedules", new[] { "UserInfo_Id" });
            DropPrimaryKey("dbo.UserInfoes");
            AlterColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserInfoes", "Id");
            DropColumn("dbo.WorkSchedules", "UserInfo_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkSchedules", "UserInfo_Id", c => c.Int());
            DropPrimaryKey("dbo.UserInfoes");
            AlterColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserInfoes", "Id");
            CreateIndex("dbo.WorkSchedules", "UserInfo_Id");
            CreateIndex("dbo.UserInfoes", "Id");
            AddForeignKey("dbo.UserInfoes", "Id", "dbo.WorkSchedules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WorkSchedules", "UserInfo_Id", "dbo.UserInfoes", "Id");
        }
    }
}
