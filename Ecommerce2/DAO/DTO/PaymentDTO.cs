namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreatePaymentVM
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public required string PaymentMethod { get; set; }
        public required string Status { get; set; }
        public required string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    // ===== UPDATE DTO =====
    public class UpdatePaymentVM
    {
        public required string Status { get; set; }
        public required string TransactionId { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class PaymentVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public required string PaymentMethod { get; set; }
        public required string Status { get; set; }
        public required string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
