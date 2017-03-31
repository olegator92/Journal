namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDebtDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Records", "DebtWorkDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Records", "DebtWorkDate", c => c.DateTime(nullable: false));
        }
    }
}
