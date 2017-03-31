namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DebtWorkDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "DebtWorkDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Records", "DebtWorkDate");
        }
    }
}
