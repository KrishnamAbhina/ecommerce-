namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateAddressVM
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }

    // ===== UPDATE DTO =====
    public class UpdateAddressVM
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class AddressVM
    {
        public int Id { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
