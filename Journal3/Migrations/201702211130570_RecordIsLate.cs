namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecordIsLate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "IsLate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Records", "IsLate");
        }
    }
}
