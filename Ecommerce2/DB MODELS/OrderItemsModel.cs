using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("OrderItems")]
    public class OrderItemsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public required string UserId { get; set; }

        [Required]
        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("ProductModel")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public UserModel? User { get; set; }
        public Orders? Order { get; set; }
        public Products? Product { get; set; }
    }
}
