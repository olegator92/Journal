namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkSchedules", "Id", "dbo.UserInfoes");
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.Records", new[] { "User_Id" });
            DropIndex("dbo.Records", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropIndex("dbo.WorkSchedules", new[] { "Id" });
            DropTable("dbo.Records");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.WorkSchedules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WorkSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        StartWork = c.Time(nullable: false, precision: 7),
                        EndWork = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Key = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        User_Id = c.String(nullable: false, maxLength: 128),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.WorkSchedules", "Id");
            CreateIndex("dbo.UserInfoes", "User_Id");
            CreateIndex("dbo.Records", "WorkSchedule_Id");
            CreateIndex("dbo.Records", "User_Id");
            AddForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WorkSchedules", "Id", "dbo.UserInfoes", "Id", cascadeDelete: true);
        }
    }
}
