namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Holidays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WorkSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
            AddColumn("dbo.SpecialSchedules", "WithoutTimeBreak", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Records", "Comment", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Records", "Comment", c => c.String());
            DropColumn("dbo.SpecialSchedules", "WithoutTimeBreak");
            DropColumn("dbo.WorkSchedules", "WithoutTimeBreak");
            DropTable("dbo.Holidays");
        }
    }
}
