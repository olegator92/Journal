namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Records", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Records", new[] { "User_Id" });
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropColumn("dbo.Records", "User_Id");
            RenameColumn(table: "dbo.Records", name: "ApplicationUser_Id", newName: "User_Id");
            AlterColumn("dbo.Records", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Records", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.UserInfoes", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Records", "User_Id");
            CreateIndex("dbo.UserInfoes", "User_Id");
            AddForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Records", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserInfoes", new[] { "User_Id" });
            DropIndex("dbo.Records", new[] { "User_Id" });
            AlterColumn("dbo.UserInfoes", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Records", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Records", "User_Id", c => c.String(maxLength: 128));
            RenameColumn(table: "dbo.Records", name: "User_Id", newName: "ApplicationUser_Id");
            AddColumn("dbo.Records", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserInfoes", "User_Id");
            CreateIndex("dbo.Records", "User_Id");
            CreateIndex("dbo.Records", "ApplicationUser_Id");
            AddForeignKey("dbo.Records", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
