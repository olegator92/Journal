namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropRecord : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropIndex("dbo.Records", new[] { "User_Id" });
            DropIndex("dbo.Records", new[] { "WorkSchedule_Id" });
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
                        User_Id = c.String(nullable: false, maxLength: 128),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Records", "WorkSchedule_Id");
            CreateIndex("dbo.Records", "User_Id");
            AddForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
