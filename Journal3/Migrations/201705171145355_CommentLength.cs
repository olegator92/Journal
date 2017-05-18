namespace Journal3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Records", "Comment", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Records", "Comment", c => c.String());
        }
    }
}
