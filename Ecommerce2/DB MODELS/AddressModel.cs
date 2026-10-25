using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Addresses")]
    public class AddressModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public required string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Street { get; set; }

        [Required]
        [MaxLength(50)]
        public required string City { get; set; }

        [Required]
        [MaxLength(50)]
        public required string State { get; set; }

        [Required]
        [MaxLength(10)]
        public required string ZipCode { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Country { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        public UserModel? User { get; set; }
        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
