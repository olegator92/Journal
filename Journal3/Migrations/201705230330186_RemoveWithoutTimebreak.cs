namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveWithoutTimebreak : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WorkSchedules", "WithoutTimeBreak");
            DropColumn("dbo.SpecialSchedules", "WithoutTimeBreak");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SpecialSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
            AddColumn("dbo.WorkSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
        }
    }
}
