using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private FashionShopDbContext _dbContext;
        public FashionShopDbContext Init()
        {
            return _dbContext ?? (_dbContext = new FashionShopDbContext());
        }
        protected override void DisposeCore()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
    }
}
