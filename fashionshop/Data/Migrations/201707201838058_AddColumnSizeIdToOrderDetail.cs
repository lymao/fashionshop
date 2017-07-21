namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnSizeIdToOrderDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "SizeId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderDetails", "SizeId");
            AddForeignKey("dbo.OrderDetails", "SizeId", "dbo.Sizes", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "SizeId", "dbo.Sizes");
            DropIndex("dbo.OrderDetails", new[] { "SizeId" });
            DropColumn("dbo.OrderDetails", "SizeId");
        }
    }
}
