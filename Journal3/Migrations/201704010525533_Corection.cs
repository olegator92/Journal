namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Corection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkSchedules", "UserInfoId", "dbo.UserInfoes");
            RenameColumn(table: "dbo.Records", name: "WorkSchedule_UserInfoId", newName: "WorkSchedule_Id");
            RenameColumn(table: "dbo.WorkSchedules", name: "UserInfoId", newName: "Id");
            RenameIndex(table: "dbo.Records", name: "IX_WorkSchedule_UserInfoId", newName: "IX_WorkSchedule_Id");
            RenameIndex(table: "dbo.WorkSchedules", name: "IX_UserInfoId", newName: "IX_Id");
            DropPrimaryKey("dbo.UserInfoes");
            DropColumn("dbo.UserInfoes", "UserInfoId");
            AddColumn("dbo.UserInfoes", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserInfoes", "Id");
            AddForeignKey("dbo.WorkSchedules", "Id", "dbo.UserInfoes", "Id", cascadeDelete: true);
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserInfoes", "UserInfoId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.WorkSchedules", "Id", "dbo.UserInfoes");
            DropPrimaryKey("dbo.UserInfoes");
            DropColumn("dbo.UserInfoes", "Id");
            AddPrimaryKey("dbo.UserInfoes", "UserInfoId");
            RenameIndex(table: "dbo.WorkSchedules", name: "IX_Id", newName: "IX_UserInfoId");
            RenameIndex(table: "dbo.Records", name: "IX_WorkSchedule_Id", newName: "IX_WorkSchedule_UserInfoId");
            RenameColumn(table: "dbo.WorkSchedules", name: "Id", newName: "UserInfoId");
            RenameColumn(table: "dbo.Records", name: "WorkSchedule_Id", newName: "WorkSchedule_UserInfoId");
            AddForeignKey("dbo.WorkSchedules", "UserInfoId", "dbo.UserInfoes", "UserInfoId", cascadeDelete: true);
        }
    }
}
