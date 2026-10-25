using DAL.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ecommerce_application.SERVICES;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        
        [HttpPost("add-to-cart")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartVM model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { error = "Cart item data is required" });
                }

                var productId = await _cartService.AddToCartAsync(model.UserId, model.ProductId, model.Quantity);

                return CreatedAtAction(nameof(GetCartItemById), new { cartItemId = productId, userId = model.UserId },
                    new { message = "Product added to cart successfully", productId = productId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error adding product to cart", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all cart items for user
        /// </summary>
        [HttpGet("all-items/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CartItemVM>>> GetAllCartItems(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                var cartItems = await _cartService.GetAllCartItemsAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching cart items", details = ex.Message });
            }
        }

        /// <summary>
        /// Get cart item by ID for user
        /// </summary>
        [HttpGet("{cartItemId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartItemVM>> GetCartItemById(int cartItemId, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                var cartItem = await _cartService.GetCartItemByIdAsync(cartItemId, userId);
                return Ok(cartItem);
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
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching cart item", details = ex.Message });
            }
        }

        /// <summary>
        /// Update cart item quantity for user
        /// </summary>
        [HttpPut("update-quantity/{cartItemId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, string userId, [FromBody] UpdateCartItemVM model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                if (model == null)
                {
                    return BadRequest(new { error = "Cart item data is required" });
                }

                var result = await _cartService.UpdateCartItemQuantityAsync(cartItemId, model.Quantity, userId);

                if (!result)
                {
                    return NotFound(new { error = $"Cart item with ID {cartItemId} not found" });
                }

                return Ok(new { message = $"Cart item quantity updated successfully" });
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
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error updating cart item quantity", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove item from cart for user
        /// </summary>
        [HttpDelete("remove/{cartItemId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFromCart(int cartItemId, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                if (cartItemId <= 0)
                {
                    return BadRequest(new { error = "Cart Item ID must be greater than 0" });
                }

                var result = await _cartService.RemoveFromCartAsync(cartItemId, userId);

                if (!result)
                {
                    return NotFound(new { error = $"Cart item with ID {cartItemId} not found" });
                }

                return Ok(new { message = "Cart item removed successfully" });
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
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error removing cart item", details = ex.Message });
            }
        }

        /// <summary>
        /// Clear all cart items for user
        /// </summary>
        [HttpDelete("clear-all/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                var result = await _cartService.ClearCartAsync(userId);

                if (!result)
                {
                    return BadRequest(new { error = "Error clearing cart" });
                }

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error clearing cart", details = ex.Message });
            }
        }

        /// <summary>
        /// Get cart summary (total items and total price) for user
        /// </summary>
        [HttpGet("summary/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartSummaryVM>> GetCartSummary(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID is required" });
                }

                var summary = await _cartService.GetCartSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching cart summary", details = ex.Message });
            }
        }
    }
}
