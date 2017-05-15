namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnImage1AndTrendFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Image1", c => c.String(maxLength: 256));
            AddColumn("dbo.Products", "TrendFlag", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "TrendFlag");
            DropColumn("dbo.Products", "Image1");
        }
    }
}
