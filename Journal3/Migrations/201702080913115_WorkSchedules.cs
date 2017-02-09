namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkSchedules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartWork = c.Time(nullable: false, precision: 7),
                        EndWork = c.Time(nullable: false, precision: 7),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.Records", "WorkSchedule_Id", c => c.Int());
            CreateIndex("dbo.Records", "WorkSchedule_Id");
            AddForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            DropColumn("dbo.UserInfoes", "StartWork");
            DropColumn("dbo.UserInfoes", "EndWork");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserInfoes", "EndWork", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.UserInfoes", "StartWork", c => c.Time(nullable: false, precision: 7));
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.WorkSchedules", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.WorkSchedules", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Records", new[] { "WorkSchedule_Id" });
            DropColumn("dbo.Records", "WorkSchedule_Id");
            DropTable("dbo.WorkSchedules");
        }
    }
}
