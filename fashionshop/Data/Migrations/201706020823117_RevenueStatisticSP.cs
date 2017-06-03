namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevenueStatisticSP : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("GetRevenueStatistic",
                m => new
                {
                    fromDate = m.String(),
                    toDate = m.String()
                },
                @"select o.CreatedDate as Date,
                sum(od.Quantity*od.Price) as Revenues,
                sum((od.Quantity*od.Price)-(od.Quantity*p.OriginalPrice)) as Benefit
                from Orders o
                inner join OrderDetails od
                on o.ID=od.OrderID
                inner join Products p
                on p.ID=od.ProductID
                where o.CreatedDate between cast(@fromDate as date) and cast(@toDate as date)
                group by o.CreatedDate"
                );
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.GetRevenueStatistic");
        }
    }
}
