using DAL.DTO;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class AuthDAO
    {
        private readonly EcommerceDbContext _context;

        public AuthDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ CREATE ============

        /// <summary>
        /// Create a new user from RegisterVM
        /// </summary>
        public async Task<string> CreateUserAsync(RegisterVM model, string hashedPassword, string role)
        {
            try
            {
                if (model == null)
                    throw new ArgumentException("User data is required.");

                if (string.IsNullOrWhiteSpace(model.Email))
                    throw new ArgumentException("Email is required.");

                if (string.IsNullOrWhiteSpace(model.FullName))
                    throw new ArgumentException("Full name is required.");

                if (string.IsNullOrWhiteSpace(hashedPassword))
                    throw new ArgumentException("Password hash is required.");

                // Map DTO to Entity
                var userId = Guid.NewGuid().ToString();
                var user = new UserModel
                {
                    Id = userId,
                    FullName = model.FullName.Trim(),
                    Email = model.Email.Trim(),
                    PhoneNumber = model.PhoneNumber?.Trim(),
                    PasswordHash = hashedPassword,
                    Role = role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return userId;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Database error: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a new refresh token
        /// </summary>
        public async Task<string> CreateRefreshTokenAsync(string userId, string token, DateTime expiryDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token is required.");

                // Map to Entity
                var refreshTokenModel = new RefreshTokenModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Token = token,
                    ExpiryDate = expiryDate,
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenModel);
                await _context.SaveChangesAsync();
                return refreshTokenModel.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating refresh token: {ex.Message}", ex);
            }
        }

        // ============ READ ============

        /// <summary>
        /// Check if user exists by email
        /// </summary>
        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email is required.");

                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking user existence: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user by email (returns entity for password verification)
        /// </summary>
        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email is required.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user by email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user by ID (returns entity)
        /// </summary>
        public async Task<UserModel> GetUserByIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user by ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get refresh token by token value
        /// </summary>
        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token is required.");

                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == token);
                return refreshToken;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting refresh token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all users as UserListVM
        /// </summary>
        public async Task<List<UserListVM>> GetAllUsersAsync()
        {
            try
            {
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
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all users: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user by ID as UserListVM
        /// </summary>
        public async Task<UserListVM> GetUserByIdAsVMAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users
                    .Where(u => u.Id == userId)
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
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user by ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user profile as UserProfileVM
        /// </summary>
        public async Task<UserProfileVM> GetUserProfileAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserProfileVM
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role
                    })
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user profile: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get active refresh tokens for user
        /// </summary>
        public async Task<List<RefreshTokenModel>> GetActiveRefreshTokensAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();
                return tokens;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting active refresh tokens: {ex.Message}", ex);
            }
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update user profile from UpdateProfileVM
        /// </summary>
        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileVM model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (model == null)
                    throw new ArgumentException("Profile data is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                // Map and update fields
                user.FullName = model.FullName ?? user.FullName;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user profile: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update user password
        /// </summary>
        public async Task<bool> UpdateUserPasswordAsync(string userId, string hashedPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (string.IsNullOrWhiteSpace(hashedPassword))
                    throw new ArgumentException("Password hash is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.PasswordHash = hashedPassword;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user password: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update refresh token
        /// </summary>
        public async Task<bool> UpdateRefreshTokenAsync(RefreshTokenModel refreshToken)
        {
            try
            {
                if (refreshToken == null)
                    throw new ArgumentException("Refresh token data is required.");

                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating refresh token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update user role
        /// </summary>
        public async Task<bool> UpdateUserRoleAsync(string userId, string newRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                if (string.IsNullOrWhiteSpace(newRole))
                    throw new ArgumentException("New role is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.Role = newRole;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user role: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deactivate user
        /// </summary>
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deactivating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Activate user
        /// </summary>
        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error activating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Revoke all refresh tokens for user
        /// </summary>
        public async Task<bool> RevokeAllRefreshTokensAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                }

                _context.RefreshTokens.UpdateRange(refreshTokens);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error revoking refresh tokens: {ex.Message}", ex);
            }
        }

        // ============ DELETE ============

        /// <summary>
        /// Delete user
        /// </summary>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("User ID is required.");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }

        // ============ STATISTICS ============

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public async Task<dynamic> GetDashboardStatsAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
                var adminCount = await _context.Users.CountAsync(u => u.Role == "admin");
                var userCount = await _context.Users.CountAsync(u => u.Role == "user");

                return new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = totalUsers - activeUsers,
                    AdminCount = adminCount,
                    UserCount = userCount
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting dashboard stats: {ex.Message}", ex);
            }
        }
    }
}
