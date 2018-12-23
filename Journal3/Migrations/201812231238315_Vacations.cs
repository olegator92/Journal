namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Vacations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vacations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "Vacation_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Vacation_Id");
            AddForeignKey("dbo.AspNetUsers", "Vacation_Id", "dbo.Vacations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Vacation_Id", "dbo.Vacations");
            DropIndex("dbo.AspNetUsers", new[] { "Vacation_Id" });
            DropColumn("dbo.AspNetUsers", "Vacation_Id");
            DropTable("dbo.Vacations");
        }
    }
}
