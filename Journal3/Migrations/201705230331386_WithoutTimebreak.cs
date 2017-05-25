namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WithoutTimebreak : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
            AddColumn("dbo.SpecialSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpecialSchedules", "WithoutTimeBreak");
            DropColumn("dbo.WorkSchedules", "WithoutTimeBreak");
        }
    }
}
