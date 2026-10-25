using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("RefreshTokens")]
    public class RefreshTokenModel
    {
        [Key]
        public required string Id { get; set; }

        [Required]
        [ForeignKey("UserModel")]
        public required string UserId { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        [Required]
        public bool IsRevoked { get; set; } = false;

        public UserModel? User { get; set; }
    }
}
