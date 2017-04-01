namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnAll : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Records",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        DateRecord = c.DateTime(nullable: false),
                        DebtWorkDate = c.DateTime(),
                        TimeRecord = c.Time(nullable: false, precision: 7),
                        Status = c.Int(nullable: false),
                        Remark = c.Int(nullable: false),
                        Comment = c.String(),
                        IsLate = c.Boolean(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsForgiven = c.Boolean(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        WorkScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkScheduleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.WorkScheduleId);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserInfoId = c.Int(nullable: false),
                        Name = c.String(),
                        Key = c.String(),
                        WorkScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkScheduleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.WorkScheduleId);
            
            CreateTable(
                "dbo.WorkSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartWork = c.Time(nullable: false, precision: 7),
                        EndWork = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Records", "WorkScheduleId", "dbo.WorkSchedules");
            DropForeignKey("dbo.Records", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInfoes", "WorkScheduleId", "dbo.WorkSchedules");
            DropIndex("dbo.UserInfoes", new[] { "WorkScheduleId" });
            DropIndex("dbo.UserInfoes", new[] { "UserId" });
            DropIndex("dbo.Records", new[] { "WorkScheduleId" });
            DropIndex("dbo.Records", new[] { "UserId" });
            DropTable("dbo.WorkSchedules");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.Records");
        }
    }
}
