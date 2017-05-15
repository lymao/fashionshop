namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Size", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Size");
        }
    }
}
