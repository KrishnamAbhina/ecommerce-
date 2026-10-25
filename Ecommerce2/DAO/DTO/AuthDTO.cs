namespace DAL.DTO
{
    // ===== REGISTRATION DTO =====
    public class RegisterVM
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string PhoneNumber { get; set; }
    }

    // ===== LOGIN DTO =====
    public class LoginVM
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    // ===== REGISTRATION RESPONSE DTO =====
    public class RegistrationResponseVM
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }

    // ===== LOGIN RESPONSE DTO =====
    public class LoginResponseVM
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string Role { get; set; }
    }

    // ===== REFRESH TOKEN DTO =====
    public class RefreshTokenVM
    {
        public required string RefreshToken { get; set; }
    }

    // ===== USER PROFILE DTO =====
    public class UserProfileVM
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }

    // ===== UPDATE PROFILE DTO =====
    public class UpdateProfileVM
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
    }

    // ===== CHANGE PASSWORD DTO =====
    public class ChangePasswordVM
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }

    // ===== ADMIN - CREATE USER DTO =====
    public class CreateUserVM
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }

    // ===== ADMIN - USER LIST DTO =====
    public class UserListVM
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    // ===== ADMIN - UPDATE USER DTO =====
    public class UpdateUserRoleVM
    {
        public required string Role { get; set; }
    }
}
