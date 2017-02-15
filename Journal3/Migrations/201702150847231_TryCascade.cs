namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers");
            AddForeignKey("dbo.UserInfoes", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
