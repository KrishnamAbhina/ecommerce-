using DAL.DTO;
using DAL.Services;

namespace ecommerce_application.SERVICES
{
    public interface IAddressService
    {
        Task<int> CreateAddressAsync(string userId, CreateAddressVM model);
        Task<List<AddressVM>> GetAllAddressesByUserAsync(string userId);
        Task<AddressVM> GetAddressByIdAsync(int id);
        Task<AddressVM> GetAddressByIdAsync(string userId, int id);
        Task<AddressVM> GetDefaultAddressByUserAsync(string userId);
        Task<bool> UpdateAddressAsync(int id, UpdateAddressVM model);
        Task<bool> UpdateAddressAsync(string userId, int id, UpdateAddressVM model);
        Task<bool> SetDefaultAddressAsync(string userId, int addressId);
        Task<bool> DeleteAddressAsync(int id);
        Task<bool> DeleteAddressAsync(string userId, int id);
        Task<bool> HardDeleteAddressAsync(int id);
        Task<bool> HardDeleteAddressAsync(string userId, int id);
        Task<int> GetAddressCountByUserAsync(string userId);
    }

    public class AddressService : IAddressService
    {
        private readonly AddressModelDAO _addressDAO;
        private readonly ILogger<AddressService> _logger;

        public AddressService(AddressModelDAO addressDAO, ILogger<AddressService> logger)
        {
            _addressDAO = addressDAO;
            _logger = logger;
        }

        /// <summary>
        /// Create a new address for user
        /// </summary>
        public async Task<int> CreateAddressAsync(string userId, CreateAddressVM model)
        {
            try
            {
                _logger.LogInformation($"AddressService: CreateAddressAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (model == null)
                {
                    throw new ArgumentException("Address data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Street))
                {
                    throw new ArgumentException("Street is required.");
                }

                if (string.IsNullOrWhiteSpace(model.City))
                {
                    throw new ArgumentException("City is required.");
                }

                if (string.IsNullOrWhiteSpace(model.State))
                {
                    throw new ArgumentException("State is required.");
                }

                if (string.IsNullOrWhiteSpace(model.ZipCode))
                {
                    throw new ArgumentException("Zip code is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Country))
                {
                    throw new ArgumentException("Country is required.");
                }

                // Business Logic: If no default address exists and IsDefault is true, set it
                if (model.IsDefault)
                {
                    var defaultAddress = await _addressDAO.GetDefaultAddressByUserAsync(userId);
                    if (defaultAddress != null)
                    {
                        throw new InvalidOperationException("User already has a default address. Unset the current default first.");
                    }
                }

                var addressId = await _addressDAO.CreateAddressAsync(userId, model);
                _logger.LogInformation($"AddressService: Address created successfully with ID {addressId} for user {userId}");

                return addressId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in CreateAddressAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Get all addresses for a user
        /// </summary>
        public async Task<List<AddressVM>> GetAllAddressesByUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AddressService: GetAllAddressesByUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var addresses = await _addressDAO.GetAllAddressesByUserAsync(userId);
                _logger.LogInformation($"AddressService: Retrieved {addresses.Count} addresses for user {userId}");

                return addresses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in GetAllAddressesByUserAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Get address by ID
        /// </summary>
        public async Task<AddressVM> GetAddressByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: GetAddressByIdAsync called with ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(id);

                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in GetAddressByIdAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get address by ID for specific user
        /// </summary>
        public async Task<AddressVM> GetAddressByIdAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: GetAddressByIdAsync called for user {userId}, ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(userId, id);

                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for user {userId}");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in GetAddressByIdAsync for user {userId}, ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get default address for user
        /// </summary>
        public async Task<AddressVM> GetDefaultAddressByUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AddressService: GetDefaultAddressByUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var address = await _addressDAO.GetDefaultAddressByUserAsync(userId);

                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Default address not found for user {userId}");
                    throw new KeyNotFoundException($"No default address found for user {userId}.");
                }

                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in GetDefaultAddressByUserAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Update an address
        /// </summary>
        public async Task<bool> UpdateAddressAsync(int id, UpdateAddressVM model)
        {
            try
            {
                _logger.LogInformation($"AddressService: UpdateAddressAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                if (model == null)
                {
                    throw new ArgumentException("Address data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Street))
                {
                    throw new ArgumentException("Street is required.");
                }

                if (string.IsNullOrWhiteSpace(model.City))
                {
                    throw new ArgumentException("City is required.");
                }

                if (string.IsNullOrWhiteSpace(model.State))
                {
                    throw new ArgumentException("State is required.");
                }

                if (string.IsNullOrWhiteSpace(model.ZipCode))
                {
                    throw new ArgumentException("Zip code is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Country))
                {
                    throw new ArgumentException("Country is required.");
                }

                var result = await _addressDAO.UpdateAddressAsync(id, model);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for update");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                _logger.LogInformation($"AddressService: Address with ID {id} updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in UpdateAddressAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Update an address for specific user
        /// </summary>
        public async Task<bool> UpdateAddressAsync(string userId, int id, UpdateAddressVM model)
        {
            try
            {
                _logger.LogInformation($"AddressService: UpdateAddressAsync called for user {userId}, ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (model == null)
                {
                    throw new ArgumentException("Address data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Street))
                {
                    throw new ArgumentException("Street is required.");
                }

                if (string.IsNullOrWhiteSpace(model.City))
                {
                    throw new ArgumentException("City is required.");
                }

                if (string.IsNullOrWhiteSpace(model.State))
                {
                    throw new ArgumentException("State is required.");
                }

                if (string.IsNullOrWhiteSpace(model.ZipCode))
                {
                    throw new ArgumentException("Zip code is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Country))
                {
                    throw new ArgumentException("Country is required.");
                }

                var result = await _addressDAO.UpdateAddressAsync(userId, id, model);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for user {userId}");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                _logger.LogInformation($"AddressService: Address with ID {id} updated for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in UpdateAddressAsync for user {userId}, ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Set an address as default for user
        /// </summary>
        public async Task<bool> SetDefaultAddressAsync(string userId, int addressId)
        {
            try
            {
                _logger.LogInformation($"AddressService: SetDefaultAddressAsync called for user {userId}, address {addressId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (addressId <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                // Business Logic: Verify address belongs to user
                var addressBelongsToUser = await _addressDAO.DoesAddressBelongToUserAsync(addressId, userId);
                if (!addressBelongsToUser)
                {
                    throw new InvalidOperationException($"Address with ID {addressId} does not belong to user {userId}.");
                }

                var result = await _addressDAO.SetDefaultAddressAsync(userId, addressId);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Failed to set default address {addressId} for user {userId}");
                    throw new KeyNotFoundException($"Address with ID {addressId} not found.");
                }

                _logger.LogInformation($"AddressService: Address {addressId} set as default for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in SetDefaultAddressAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Soft delete an address
        /// </summary>
        public async Task<bool> DeleteAddressAsync(int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: DeleteAddressAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(id);
                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for deletion");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                // Business Logic: Cannot delete if it's the default address
                if (address.IsDefault)
                {
                    throw new InvalidOperationException("Cannot delete the default address. Set another address as default first.");
                }

                var result = await _addressDAO.DeleteAddressAsync(id);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Failed to delete address {id}");
                    return false;
                }

                _logger.LogInformation($"AddressService: Address with ID {id} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in DeleteAddressAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Soft delete an address for specific user
        /// </summary>
        public async Task<bool> DeleteAddressAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: DeleteAddressAsync called for user {userId}, ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(userId, id);
                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for user {userId}");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                // Business Logic: Cannot delete if it's the default address
                if (address.IsDefault)
                {
                    throw new InvalidOperationException("Cannot delete the default address. Set another address as default first.");
                }

                var result = await _addressDAO.DeleteAddressAsync(userId, id);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Failed to delete address {id} for user {userId}");
                    return false;
                }

                _logger.LogInformation($"AddressService: Address with ID {id} deleted for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in DeleteAddressAsync for user {userId}, ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Hard delete an address
        /// </summary>
        public async Task<bool> HardDeleteAddressAsync(int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: HardDeleteAddressAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(id);
                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for hard deletion");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                // Business Logic: Cannot hard delete if it's the default address
                if (address.IsDefault)
                {
                    throw new InvalidOperationException("Cannot hard delete the default address. Set another address as default first.");
                }

                var result = await _addressDAO.HardDeleteAddressAsync(id);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Failed to hard delete address {id}");
                    return false;
                }

                _logger.LogInformation($"AddressService: Address with ID {id} hard deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in HardDeleteAddressAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Hard delete an address for specific user
        /// </summary>
        public async Task<bool> HardDeleteAddressAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"AddressService: HardDeleteAddressAsync called for user {userId}, ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Address ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var address = await _addressDAO.GetAddressByIdAsync(userId, id);
                if (address == null)
                {
                    _logger.LogWarning($"AddressService: Address with ID {id} not found for user {userId}");
                    throw new KeyNotFoundException($"Address with ID {id} not found.");
                }

                // Business Logic: Cannot hard delete if it's the default address
                if (address.IsDefault)
                {
                    throw new InvalidOperationException("Cannot hard delete the default address. Set another address as default first.");
                }

                var result = await _addressDAO.HardDeleteAddressAsync(userId, id);

                if (!result)
                {
                    _logger.LogWarning($"AddressService: Failed to hard delete address {id} for user {userId}");
                    return false;
                }

                _logger.LogInformation($"AddressService: Address with ID {id} hard deleted for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in HardDeleteAddressAsync for user {userId}, ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get address count for user
        /// </summary>
        public async Task<int> GetAddressCountByUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"AddressService: GetAddressCountByUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var count = await _addressDAO.GetAddressCountByUserAsync(userId);
                _logger.LogInformation($"AddressService: User {userId} has {count} active addresses");

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddressService: Error in GetAddressCountByUserAsync for user {userId}");
                throw;
            }
        }
    }
}
