using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Carts")]
    public class CartModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public required string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        public UserModel? User { get; set; }
        public ICollection<CartItemsModel> CartItems { get; set; } = new List<CartItemsModel>();
    }
}

