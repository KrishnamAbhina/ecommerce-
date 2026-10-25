using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public required string Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public UserModel? User { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
