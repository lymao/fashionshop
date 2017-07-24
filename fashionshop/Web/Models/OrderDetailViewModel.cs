using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class OrderDetailViewModel
    {
        public int OrderID { set; get; }

        public int ProductID { set; get; }

        public int Quantity { set; get; }

        public decimal Price { set; get; }

        public int SizeID { get; set; }

        public ProductViewModel Product { get; set; }

        public SizeViewModel Size { set; get; }
    }
}