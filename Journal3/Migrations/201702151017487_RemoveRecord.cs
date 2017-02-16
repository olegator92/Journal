namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRecord : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.Records", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Records", new[] { "User_Id" });
            DropIndex("dbo.Records", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            AlterColumn("dbo.UserInfoes", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UserInfoes", "User_Id");
            DropTable("dbo.Records");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Records",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        DateRecord = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.Int(nullable: false),
                        Comment = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsForgiven = c.Boolean(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            AlterColumn("dbo.UserInfoes", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserInfoes", "User_Id");
            CreateIndex("dbo.Records", "WorkSchedule_Id");
            CreateIndex("dbo.Records", "User_Id");
            CreateIndex("dbo.Records", "ApplicationUser_Id");
            AddForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
