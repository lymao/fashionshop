namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnPhoneToFeedback : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "Phone", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "Phone");
        }
    }
}
