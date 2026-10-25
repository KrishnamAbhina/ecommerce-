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
    public class AddressModelDAO
    {
        private readonly EcommerceDbContext _context;

        public AddressModelDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ CREATE ============

        /// <summary>
        /// Create a new address
        /// </summary>
        public async Task<int> CreateAddressAsync(string userId, CreateAddressVM model)
        {
            var address = new AddressModel
            {
                UserId = userId,
                Street = model.Street,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber,
                IsDefault = model.IsDefault,
                IsActive = true
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return address.Id;
        }

        // ============ READ ============

        /// <summary>
        /// Get all addresses for a user
        /// </summary>
        public async Task<List<AddressVM>> GetAllAddressesByUserAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .Select(a => new AddressVM
                {
                    Id = a.Id,
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                    PhoneNumber = a.PhoneNumber,
                    IsDefault = a.IsDefault,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return addresses;
        }

        /// <summary>
        /// Get address by ID
        /// </summary>
        public async Task<AddressVM> GetAddressByIdAsync(int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);

            if (address == null)
            {
                return null;
            }

            return new AddressVM
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive
            };
        }

        /// <summary>
        /// Get address by ID for specific user
        /// </summary>
        public async Task<AddressVM> GetAddressByIdAsync(string userId, int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return null;
            }

            return new AddressVM
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive
            };
        }

        /// <summary>
        /// Get default address for a user
        /// </summary>
        public async Task<AddressVM> GetDefaultAddressByUserAsync(string userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault && a.IsActive);

            if (address == null)
            {
                return null;
            }

            return new AddressVM
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive
            };
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update an address
        /// </summary>
        public async Task<bool> UpdateAddressAsync(int id, UpdateAddressVM model)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return false;
            }

            address.Street = model.Street;
            address.City = model.City;
            address.State = model.State;
            address.ZipCode = model.ZipCode;
            address.Country = model.Country;
            address.PhoneNumber = model.PhoneNumber;
            address.IsDefault = model.IsDefault;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Update an address for specific user
        /// </summary>
        public async Task<bool> UpdateAddressAsync(string userId, int id, UpdateAddressVM model)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            address.Street = model.Street;
            address.City = model.City;
            address.State = model.State;
            address.ZipCode = model.ZipCode;
            address.Country = model.Country;
            address.PhoneNumber = model.PhoneNumber;
            address.IsDefault = model.IsDefault;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Set address as default for user
        /// </summary>
        public async Task<bool> SetDefaultAddressAsync(string userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            // Unset previous default
            var previousDefault = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

            if (previousDefault != null)
            {
                previousDefault.IsDefault = false;
                _context.Addresses.Update(previousDefault);
            }

            // Set new default
            address.IsDefault = true;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ DELETE ============

        /// <summary>
        /// Soft delete an address (set IsActive to false)
        /// </summary>
        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return false;
            }

            address.IsActive = false;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Soft delete an address for specific user
        /// </summary>
        public async Task<bool> DeleteAddressAsync(string userId, int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            address.IsActive = false;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Hard delete an address
        /// </summary>
        public async Task<bool> HardDeleteAddressAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return false;
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Hard delete an address for specific user
        /// </summary>
        public async Task<bool> HardDeleteAddressAsync(string userId, int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ ADDITIONAL METHODS ============

        /// <summary>
        /// Get count of active addresses for a user
        /// </summary>
        public async Task<int> GetAddressCountByUserAsync(string userId)
        {
            var count = await _context.Addresses
                .CountAsync(a => a.UserId == userId && a.IsActive);

            return count;
        }

        /// <summary>
        /// Check if address belongs to user
        /// </summary>
        public async Task<bool> DoesAddressBelongToUserAsync(int addressId, string userId)
        {
            var exists = await _context.Addresses
                .AnyAsync(a => a.Id == addressId && a.UserId == userId);

            return exists;
        }
    }
}
