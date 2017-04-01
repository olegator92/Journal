namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Correction : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserInfoes", "UserId");
            DropColumn("dbo.UserInfoes", "WorkScheduleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserInfoes", "WorkScheduleId", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "UserId", c => c.String());
        }
    }
}
