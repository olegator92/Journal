namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduleReform : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpecialScheduleWorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules");
            DropForeignKey("dbo.SpecialScheduleWorkSchedules", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.SpecialScheduleWorkSchedules", new[] { "SpecialSchedule_Id" });
            DropIndex("dbo.SpecialScheduleWorkSchedules", new[] { "WorkSchedule_Id" });
            AddColumn("dbo.SpecialSchedules", "WorkScheduleId", c => c.Int(nullable: false));
            CreateIndex("dbo.SpecialSchedules", "WorkScheduleId");
            AddForeignKey("dbo.SpecialSchedules", "WorkScheduleId", "dbo.WorkSchedules", "Id", cascadeDelete: true);
            DropTable("dbo.SpecialScheduleWorkSchedules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SpecialScheduleWorkSchedules",
                c => new
                    {
                        SpecialSchedule_Id = c.Int(nullable: false),
                        WorkSchedule_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SpecialSchedule_Id, t.WorkSchedule_Id });
            
            DropForeignKey("dbo.SpecialSchedules", "WorkScheduleId", "dbo.WorkSchedules");
            DropIndex("dbo.SpecialSchedules", new[] { "WorkScheduleId" });
            DropColumn("dbo.SpecialSchedules", "WorkScheduleId");
            CreateIndex("dbo.SpecialScheduleWorkSchedules", "WorkSchedule_Id");
            CreateIndex("dbo.SpecialScheduleWorkSchedules", "SpecialSchedule_Id");
            AddForeignKey("dbo.SpecialScheduleWorkSchedules", "WorkSchedule_Id", "dbo.WorkSchedules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SpecialScheduleWorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules", "Id", cascadeDelete: true);
        }
    }
}
