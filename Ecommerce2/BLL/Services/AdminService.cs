using DAL.DTO;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ecommerce_application.SERVICES
{
    public interface IAdminService
    {
        Task<List<UserListVM>> GetAllUsersAsync();
        Task<UserListVM> GetUserByIdAsync(string userId);
        Task<int> CreateUserAsync(CreateUserVM model);
        Task<bool> UpdateUserRoleAsync(string userId, UpdateUserRoleVM model);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<dynamic> GetAdminDashboardStatsAsync();
    }

    public class AdminService : IAdminService
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(EcommerceDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserListVM>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("AdminService: GetAllUsersAsync called");

                var users = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserListVM
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role,
                        CreatedAt = u.CreatedAt,
                        IsActive = u.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation($"AdminService: Retrieved {users.Count} users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in GetAllUsersAsync");
                throw;
            }
        }

        public async Task<UserListVM> GetUserByIdAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AdminService: GetUserByIdAsync called for user {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                return new UserListVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in GetUserByIdAsync");
                throw;
            }
        }

        public async Task<int> CreateUserAsync(CreateUserVM model)
        {
            try
            {
                _logger.LogInformation($"AdminService: CreateUserAsync called for email {model.Email}");

                if (string.IsNullOrWhiteSpace(model.Email))
                    throw new ArgumentException("Email is required.");

                var userExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (userExists)
                    throw new ArgumentException("Email already exists.");

                var userId = Guid.NewGuid().ToString();
                var hashedPassword = HashPassword(model.Password);

                var user = new UserModel
                {
                    Id = userId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = hashedPassword,
                    Role = model.Role ?? "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"AdminService: User created successfully with ID {userId}");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in CreateUserAsync");
                throw;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, UpdateUserRoleVM model)
        {
            try
            {
                _logger.LogInformation($"AdminService: UpdateUserRoleAsync called for user {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                var validRoles = new[] { "user", "admin", "moderator" };
                if (!validRoles.Contains(model.Role.ToLower()))
                    throw new ArgumentException($"Invalid role. Valid roles are: {string.Join(", ", validRoles)}");

                user.Role = model.Role.ToLower();
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"AdminService: Role updated for user {userId} to {model.Role}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in UpdateUserRoleAsync");
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AdminService: DeactivateUserAsync called for user {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"AdminService: User {userId} deactivated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in DeactivateUserAsync");
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AdminService: ActivateUserAsync called for user {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"AdminService: User {userId} activated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in ActivateUserAsync");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AdminService: DeleteUserAsync called for user {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                var refreshTokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();

                if (refreshTokens.Any())
                    _context.RefreshTokens.RemoveRange(refreshTokens);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"AdminService: User {userId} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in DeleteUserAsync");
                throw;
            }
        }

        public async Task<dynamic> GetAdminDashboardStatsAsync()
        {
            try
            {
                _logger.LogInformation("AdminService: GetAdminDashboardStatsAsync called");

                var totalUsers = await _context.Users.CountAsync();
                var totalOrders = await _context.Orders.CountAsync();
                var totalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
                var totalProducts = await _context.Products.CountAsync();
                var adminCount = await _context.Users.CountAsync(u => u.Role == "admin");
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);

                return new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    TotalProducts = totalProducts,
                    AdminCount = adminCount,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error in GetAdminDashboardStatsAsync");
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
