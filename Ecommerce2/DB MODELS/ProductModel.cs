using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 3,
            ErrorMessage = "Product name must be between 3 and 150 characters.")]
        public required string Name { get; set; }

        [StringLength(1000,
            ErrorMessage = "Description cannot exceed 1000 characters.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [StringLength(500,
            ErrorMessage = "Image path cannot exceed 500 characters.")]
        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }



        public UserModel? User { get; set; }
        public virtual Categories? Category { get; set; }
        public virtual ICollection<CartItemsModel> CartItems { get; set; } = new List<CartItemsModel>();
        public virtual ICollection<OrderItemsModel> OrderItems { get; set; } = new List<OrderItemsModel>();
    }
}