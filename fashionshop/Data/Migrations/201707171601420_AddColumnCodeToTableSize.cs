namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnCodeToTableSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sizes", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sizes", "Code");
        }
    }
}
