using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("CartItems")]
    public class CartItemsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public required string UserId { get; set; }

        [ForeignKey("Cart")]
        public int? CartId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public UserModel? User { get; set; }
        public Products? Product { get; set; }
    }
}
