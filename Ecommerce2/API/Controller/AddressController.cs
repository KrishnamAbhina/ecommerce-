using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ecommerce_application.SERVICES;
using DAL.DTO;
using System.Security.Claims;

namespace Ecommerce2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private string GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressVM model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var addressId = await _addressService.CreateAddressAsync(userId, model);
                return CreatedAtAction(nameof(GetAddressById), new { addressId },
                    new { id = addressId, message = "Address created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't create the address", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var addresses = await _addressService.GetAllAddressesByUserAsync(userId);
                return Ok(addresses);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load addresses", details = ex.Message });
            }
        }

        [HttpGet("id/{addressId}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var address = await _addressService.GetAddressByIdAsync(userId, addressId);
                return Ok(address);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load the address", details = ex.Message });
            }
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var address = await _addressService.GetDefaultAddressByUserAsync(userId);
                return Ok(address);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load the default address", details = ex.Message });
            }
        }

        [HttpPut("id/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressVM model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _addressService.UpdateAddressAsync(userId, addressId, model);

                if (!result)
                {
                    return NotFound(new { error = $"Address with ID {addressId} not found" });
                }

                return Ok(new { message = $"Address with ID {addressId} updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't update the address", details = ex.Message });
            }
        }

        [HttpPatch("id/{addressId}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _addressService.SetDefaultAddressAsync(userId, addressId);

                if (!result)
                {
                    return NotFound(new { error = $"Address with ID {addressId} not found" });
                }

                return Ok(new { message = $"Address with ID {addressId} set as default" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't set the default address", details = ex.Message });
            }
        }

        [HttpDelete("id/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _addressService.DeleteAddressAsync(userId, addressId);

                if (!result)
                {
                    return NotFound(new { error = $"Address with ID {addressId} not found" });
                }

                return Ok(new { message = $"Address with ID {addressId} deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't delete the address", details = ex.Message });
            }
        }

        [HttpDelete("id/{addressId}/hard-delete")]
        public async Task<IActionResult> HardDeleteAddress(int addressId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _addressService.HardDeleteAddressAsync(userId, addressId);

                if (!result)
                {
                    return NotFound(new { error = $"Address with ID {addressId} not found" });
                }

                return Ok(new { message = $"Address with ID {addressId} permanently deleted" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't permanently delete the address", details = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetAddressCount()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var count = await _addressService.GetAddressCountByUserAsync(userId);
                return Ok(new { count });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't get address count", details = ex.Message });
            }
        }
    }
}
