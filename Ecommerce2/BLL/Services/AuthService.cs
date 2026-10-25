using DAL.DTO;
using DAL.Services;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace ecommerce_application.SERVICES
{
    public interface IAuthService
    {
        // Registration
        Task<RegistrationResponseVM> RegisterUserAsync(RegisterVM model);
        Task<RegistrationResponseVM> RegisterAdminAsync(RegisterVM model);

        // Login
        Task<LoginResponseVM> LoginAsync(LoginVM model);

        // Token Management
        Task<LoginResponseVM> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string userId);

        // Profile Management
        Task<UserProfileVM> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileVM model);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordVM model);

        // User/Admin Management
        Task<List<UserListVM>> GetAllUsersAsync();
        Task<UserListVM> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserRoleAsync(string userId, UpdateUserRoleVM model);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<dynamic> GetDashboardStatsAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly AuthDAO _authDAO;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AuthDAO authDAO, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _authDAO = authDAO;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<RegistrationResponseVM> RegisterUserAsync(RegisterVM model)
        {
            return await RegisterInternalAsync(model, "user");
        }

        public async Task<RegistrationResponseVM> RegisterAdminAsync(RegisterVM model)
        {
            return await RegisterInternalAsync(model, "admin");
        }

        private async Task<RegistrationResponseVM> RegisterInternalAsync(RegisterVM model, string role)
        {
            try
            {
                _logger.LogInformation($"AuthService: RegisterAsync called for email {model.Email}");

                // Validation (Business Logic Layer)
                if (string.IsNullOrWhiteSpace(model.Email))
                    throw new ArgumentException("Email is required.");

                if (string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Password is required.");

                if (model.Password != model.ConfirmPassword)
                    throw new ArgumentException("Passwords do not match.");

                if (model.Password.Length < 6)
                    throw new ArgumentException("Password must be at least 6 characters.");

                // Use DAO to check if user exists
                var userExists = await _authDAO.UserExistsByEmailAsync(model.Email);
                if (userExists)
                    throw new ArgumentException("Email already registered.");

                // Hash password
                var hashedPassword = HashPassword(model.Password);

                // Use DAO to create user
                var userId = await _authDAO.CreateUserAsync(model, hashedPassword, role);

                _logger.LogInformation($"AuthService: User registered successfully with ID {userId}");

                return new RegistrationResponseVM
                {
                    Id = userId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in RegisterAsync");
                throw;
            }
        }

        public async Task<LoginResponseVM> LoginAsync(LoginVM model)
        {
            try
            {
                _logger.LogInformation($"AuthService: LoginAsync called for email {model.Email}");

                // Validation
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Email and password are required.");

                // Use DAO to get user
                var user = await _authDAO.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning($"AuthService: Login failed - User not found for email {model.Email}");
                    throw new ArgumentException("Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"AuthService: Login failed - User is inactive for email {model.Email}");
                    throw new ArgumentException("User account is inactive.");
                }

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    _logger.LogError($"AuthService: PasswordHash is null for user {user.Id}");
                    throw new ArgumentException("Invalid email or password.");
                }

                if (!VerifyPassword(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"AuthService: Invalid password for user {user.Email}");
                    throw new ArgumentException("Invalid email or password.");
                }

                // Generate tokens (Business Logic)
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                // Use DAO to create refresh token
                await _authDAO.CreateRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

                _logger.LogInformation($"AuthService: User {model.Email} logged in successfully");

                return new LoginResponseVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in LoginAsync");
                throw;
            }
        }

        public async Task<LoginResponseVM> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                _logger.LogInformation("AuthService: RefreshTokenAsync called");

                // Validation
                if (string.IsNullOrWhiteSpace(refreshToken))
                    throw new ArgumentException("Refresh token is required.");

                // Use DAO to get refresh token
                var refreshTokenModel = await _authDAO.GetRefreshTokenAsync(refreshToken);

                if (refreshTokenModel == null || refreshTokenModel.ExpiryDate < DateTime.UtcNow)
                    throw new ArgumentException("Invalid or expired refresh token.");

                var user = refreshTokenModel.User;
                if (!user.IsActive)
                    throw new ArgumentException("User is inactive.");

                // Generate new tokens (Business Logic)
                var newAccessToken = GenerateAccessToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Revoke old token using DAO
                refreshTokenModel.IsRevoked = true;
                await _authDAO.UpdateRefreshTokenAsync(refreshTokenModel);

                // Create new token using DAO
                await _authDAO.CreateRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7));

                return new LoginResponseVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in RefreshTokenAsync");
                throw;
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: LogoutAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                // Use DAO to revoke all tokens
                var result = await _authDAO.RevokeAllRefreshTokensAsync(userId);

                _logger.LogInformation($"AuthService: User {userId} logged out successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in LogoutAsync");
                throw;
            }
        }

        public async Task<UserProfileVM> GetProfileAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: GetProfileAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                // Use DAO to get profile
                var profile = await _authDAO.GetUserProfileAsync(userId);
                if (profile == null)
                    throw new KeyNotFoundException("User not found.");

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in GetProfileAsync");
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileVM model)
        {
            try
            {
                _logger.LogInformation($"AuthService: UpdateProfileAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (model == null)
                    throw new ArgumentException("Profile data is required.");

                // Use DAO to update profile
                var result = await _authDAO.UpdateUserProfileAsync(userId, model);

                _logger.LogInformation($"AuthService: Profile updated for user {userId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in UpdateProfileAsync");
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordVM model)
        {
            try
            {
                _logger.LogInformation($"AuthService: ChangePasswordAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (model == null)
                    throw new ArgumentException("Password data is required.");

                if (model.NewPassword != model.ConfirmPassword)
                    throw new ArgumentException("Passwords do not match.");

                if (model.NewPassword.Length < 6)
                    throw new ArgumentException("Password must be at least 6 characters.");

                // Use DAO to get user
                var user = await _authDAO.GetUserByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                if (!VerifyPassword(model.CurrentPassword, user.PasswordHash))
                    throw new ArgumentException("Current password is incorrect.");

                // Hash new password
                var hashedPassword = HashPassword(model.NewPassword);

                // Use DAO to update password
                var result = await _authDAO.UpdateUserPasswordAsync(userId, hashedPassword);

                _logger.LogInformation($"AuthService: Password changed for user {userId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in ChangePasswordAsync");
                throw;
            }
        }

        private string GenerateAccessToken(UserModel user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new InvalidOperationException("Jwt:Issuer is not configured in appsettings.json");
            if (string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("Jwt:Audience is not configured in appsettings.json");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("userId", user.Id ?? ""),
                new System.Security.Claims.Claim("email", user.Email ?? ""),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role ?? "user")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.");

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash cannot be null or empty.");

            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        public async Task<List<UserListVM>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("AuthService: GetAllUsersAsync called");

                // Use DAO to get all users
                var users = await _authDAO.GetAllUsersAsync();

                _logger.LogInformation($"AuthService: Retrieved {users.Count} users/admins");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in GetAllUsersAsync");
                throw;
            }
        }

        public async Task<UserListVM> GetUserByIdAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: GetUserByIdAsync called for user {userId}");

                // Use DAO to get user
                var user = await _authDAO.GetUserByIdAsVMAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in GetUserByIdAsync");
                throw;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, UpdateUserRoleVM model)
        {
            try
            {
                _logger.LogInformation($"AuthService: UpdateUserRoleAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (model == null || string.IsNullOrWhiteSpace(model.Role))
                    throw new ArgumentException("Role is required.");

                var validRoles = new[] { "user", "admin" };
                if (!validRoles.Contains(model.Role.ToLower()))
                    throw new ArgumentException($"Invalid role. Valid roles are: user, admin");

                // Normalize to lowercase
                var normalizedRole = model.Role.ToLower();

                // Use DAO to update role
                var result = await _authDAO.UpdateUserRoleAsync(userId, normalizedRole);

                _logger.LogInformation($"AuthService: Role updated for user {userId} to {normalizedRole}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in UpdateUserRoleAsync");
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: DeactivateUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                // Use DAO to deactivate user
                var result = await _authDAO.DeactivateUserAsync(userId);

                _logger.LogInformation($"AuthService: User {userId} deactivated");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in DeactivateUserAsync");
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: ActivateUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                // Use DAO to activate user
                var result = await _authDAO.ActivateUserAsync(userId);

                _logger.LogInformation($"AuthService: User {userId} activated");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in ActivateUserAsync");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AuthService: DeleteUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                // Use DAO to delete user
                var result = await _authDAO.DeleteUserAsync(userId);

                _logger.LogInformation($"AuthService: User {userId} deleted");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in DeleteUserAsync");
                throw;
            }
        }

        public async Task<dynamic> GetDashboardStatsAsync()
        {
            try
            {
                _logger.LogInformation("AuthService: GetDashboardStatsAsync called");

                // Use DAO to get user statistics
                var userStats = await _authDAO.GetDashboardStatsAsync();
                return userStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService: Error in GetDashboardStatsAsync");
                throw;
            }
        }
    }
}
