using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ProductSizeViewModel
    {
        public int ProductId { get; set; }

        public int SizeId { get; set; }

        public int Quantity { set; get; }

        public virtual SizeViewModel Size { get; set; }

    }
}