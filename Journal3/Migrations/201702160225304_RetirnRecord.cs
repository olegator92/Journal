namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetirnRecord : DbMigration
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
                        Status = c.Int(nullable: false),
                        Remark = c.Int(nullable: false),
                        Comment = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsForgiven = c.Boolean(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                        WorkSchedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.WorkSchedules", t => t.WorkSchedule_Id)
                .Index(t => t.User_Id)
                .Index(t => t.WorkSchedule_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Records", new[] { "WorkSchedule_Id" });
            DropIndex("dbo.Records", new[] { "User_Id" });
            DropTable("dbo.Records");
        }
    }
}
