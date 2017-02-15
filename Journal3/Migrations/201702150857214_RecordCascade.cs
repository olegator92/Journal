namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecordCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            AddColumn("dbo.Records", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Records", "ApplicationUser_Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Records", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Records", "ApplicationUser_Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
