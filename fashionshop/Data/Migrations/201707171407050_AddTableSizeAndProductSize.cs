namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableSizeAndProductSize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSizes",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductId, t.SizeId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.SizeId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSizes", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.ProductSizes", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductSizes", new[] { "SizeId" });
            DropIndex("dbo.ProductSizes", new[] { "ProductId" });
            DropTable("dbo.Sizes");
            DropTable("dbo.ProductSizes");
        }
    }
}
