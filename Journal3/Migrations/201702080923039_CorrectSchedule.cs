namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectSchedule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkSchedules", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.WorkSchedules", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.WorkSchedules", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkSchedules", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.WorkSchedules", "ApplicationUser_Id");
            AddForeignKey("dbo.WorkSchedules", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
