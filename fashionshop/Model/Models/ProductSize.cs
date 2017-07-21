using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    [Table("ProductSizes")]
    public class ProductSize
    {
        [Key]
        [Column(Order=1)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int SizeId { get; set; }

        public int Quantity { set; get; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("SizeId")]
        public virtual Size Size { get; set; }
    }
}
