namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableWorkschedule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "WorkScheduleId", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "WorkScheduleId" });
            AlterColumn("dbo.UserInfoes", "WorkScheduleId", c => c.Int());
            CreateIndex("dbo.UserInfoes", "WorkScheduleId");
            AddForeignKey("dbo.UserInfoes", "WorkScheduleId", "dbo.WorkSchedules", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "WorkScheduleId", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "WorkScheduleId" });
            AlterColumn("dbo.UserInfoes", "WorkScheduleId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserInfoes", "WorkScheduleId");
            AddForeignKey("dbo.UserInfoes", "WorkScheduleId", "dbo.WorkSchedules", "Id", cascadeDelete: true);
        }
    }
}
