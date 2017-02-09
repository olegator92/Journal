namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryToUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "RollNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "RollNo", c => c.Int(nullable: false));
        }
    }
}
