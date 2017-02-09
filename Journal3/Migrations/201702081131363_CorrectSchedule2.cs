namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectSchedule2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "WorkSchedule_Id", c => c.Int());
            CreateIndex("dbo.UserInfoes", "WorkSchedule_Id");
            AddForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "WorkSchedule_Id" });
            DropColumn("dbo.UserInfoes", "WorkSchedule_Id");
        }
    }
}
