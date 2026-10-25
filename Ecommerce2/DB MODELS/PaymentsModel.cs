using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_application.MODELS.DB_MODELS
{
    [Table("Payments")]
    public class PaymentsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public required string PaymentMethod { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }

        [Required]
        [MaxLength(100)]
        public required string TransactionId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        public Orders? Order { get; set; }
    }
}
