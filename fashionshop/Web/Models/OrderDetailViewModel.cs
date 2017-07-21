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

        public int Quantitty { set; get; }

        public int SizeID { get; set; }

        public OrderViewModel Order { get; set; }

        public ProductViewModel Product { get; set; }

        public SizeViewModel Size { set; get; }
    }
}