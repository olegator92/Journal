namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HolidaysComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Holidays", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Holidays", "Comment");
        }
    }
}
