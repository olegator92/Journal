namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingAllowEditVacations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "AllowEditVacation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "AllowEditVacation");
        }
    }
}
