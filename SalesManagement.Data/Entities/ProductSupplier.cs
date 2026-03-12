using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("ProductSuppliers")]
    public class ProductSupplier
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }
    }
}