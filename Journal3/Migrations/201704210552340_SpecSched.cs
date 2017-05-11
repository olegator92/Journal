namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecSched : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules");
            DropIndex("dbo.WorkSchedules", new[] { "SpecialSchedule_Id" });
            CreateTable(
                "dbo.SpecialScheduleWorkSchedules",
                c => new
                    {
                        SpecialSchedule_Id = c.Int(nullable: false),
                        WorkSchedule_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SpecialSchedule_Id, t.WorkSchedule_Id })
                .ForeignKey("dbo.SpecialSchedules", t => t.SpecialSchedule_Id, cascadeDelete: true)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkSchedule_Id, cascadeDelete: true)
                .Index(t => t.SpecialSchedule_Id)
                .Index(t => t.WorkSchedule_Id);
            
            DropColumn("dbo.WorkSchedules", "SpecialSchedule_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkSchedules", "SpecialSchedule_Id", c => c.Int());
            DropForeignKey("dbo.SpecialScheduleWorkSchedules", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.SpecialScheduleWorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules");
            DropIndex("dbo.SpecialScheduleWorkSchedules", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.SpecialScheduleWorkSchedules", new[] { "SpecialSchedule_Id" });
            DropTable("dbo.SpecialScheduleWorkSchedules");
            CreateIndex("dbo.WorkSchedules", "SpecialSchedule_Id");
            AddForeignKey("dbo.WorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules", "Id", cascadeDelete: true);
        }
    }
}
