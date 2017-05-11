namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecialWorkSchedule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpecialSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DayOfWeek = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WorkSchedules", "isSpecial", c => c.Boolean(nullable: false));
            AddColumn("dbo.WorkSchedules", "SpecialSchedule_Id", c => c.Int());
            CreateIndex("dbo.WorkSchedules", "SpecialSchedule_Id");
            AddForeignKey("dbo.WorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkSchedules", "SpecialSchedule_Id", "dbo.SpecialSchedules");
            DropIndex("dbo.WorkSchedules", new[] { "SpecialSchedule_Id" });
            DropColumn("dbo.WorkSchedules", "SpecialSchedule_Id");
            DropColumn("dbo.WorkSchedules", "isSpecial");
            DropTable("dbo.SpecialSchedules");
        }
    }
}
