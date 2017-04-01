namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules");
            RenameColumn(table: "dbo.Records", name: "WorkSchedule_Id", newName: "WorkSchedule_UserInfoId");
            RenameIndex(table: "dbo.Records", name: "IX_WorkSchedule_Id", newName: "IX_WorkSchedule_UserInfoId");
            DropPrimaryKey("dbo.UserInfoes");
            DropPrimaryKey("dbo.WorkSchedules");
            DropColumn("dbo.UserInfoes", "Id");
            DropColumn("dbo.WorkSchedules", "Id");
            AddColumn("dbo.UserInfoes", "UserInfoId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.WorkSchedules", "UserInfoId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserInfoes", "UserInfoId");
            AddPrimaryKey("dbo.WorkSchedules", "UserInfoId");
            CreateIndex("dbo.WorkSchedules", "UserInfoId");
            AddForeignKey("dbo.WorkSchedules", "UserInfoId", "dbo.UserInfoes", "UserInfoId", cascadeDelete: true);
            AddForeignKey("dbo.Records", "WorkSchedule_UserInfoId", "dbo.WorkSchedules", "UserInfoId");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkSchedules", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Records", "WorkSchedule_UserInfoId", "dbo.WorkSchedules");
            DropForeignKey("dbo.WorkSchedules", "UserInfoId", "dbo.UserInfoes");
            DropIndex("dbo.WorkSchedules", new[] { "UserInfoId" });
            DropPrimaryKey("dbo.WorkSchedules");
            DropPrimaryKey("dbo.UserInfoes");
            DropColumn("dbo.WorkSchedules", "UserInfoId");
            DropColumn("dbo.UserInfoes", "UserInfoId");
            AddPrimaryKey("dbo.WorkSchedules", "Id");
            AddPrimaryKey("dbo.UserInfoes", "Id");
            RenameIndex(table: "dbo.Records", name: "IX_WorkSchedule_UserInfoId", newName: "IX_WorkSchedule_Id");
            RenameColumn(table: "dbo.Records", name: "WorkSchedule_UserInfoId", newName: "WorkSchedule_Id");
            AddForeignKey("dbo.Records", "WorkSchedule_Id", "dbo.WorkSchedules", "Id");
        }
    }
}
