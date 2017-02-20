namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimeToRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "TimeRecord", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Records", "TimeRecord");
        }
    }
}
