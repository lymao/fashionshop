using Data.Infrastructure;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IProductSizeRepository : IRepository<ProductSize>
    {
    }
    public class ProductSizeRepository : RepositoryBase<ProductSize>, IProductSizeRepository
    {
        public ProductSizeRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
