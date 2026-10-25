using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; } = "placed";

        public UserModel? User { get; set; }

        public AddressModel? Address { get; set; }
        public ICollection<OrderItemsModel> OrderItems { get; set; } = new List<OrderItemsModel>();
        public ICollection<PaymentsModel> Payments { get; set; } = new List<PaymentsModel>();
    }
}
