using System.ComponentModel.DataAnnotations;
using ecommerce_application.MODELS.DB_MODELS;

public class UserModel
{
    [Key]
    public required string Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "user";

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<AddressModel> Addresses { get; set; } = new List<AddressModel>();
    public virtual ICollection<CartModel> Carts { get; set; } = new List<CartModel>();
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
    public virtual ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
}