using DAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ecommerce_application.SERVICES;

namespace Ecommerce2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InventoryVM>>> GetAllInventory()
        {
            try
            {
                var inventories = await _inventoryService.GetAllInventoryAsync();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching inventory", details = ex.Message });
            }
        }

        [HttpGet("product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InventoryVM>> GetInventoryByProductId(int productId)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
                return Ok(inventory);
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
                    new { error = "Error fetching inventory", details = ex.Message });
            }
        }

        [HttpGet("low-stock/{threshold}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InventoryVM>>> GetLowStockInventory(int threshold = 10)
        {
            try
            {
                var inventories = await _inventoryService.GetLowStockInventoryAsync(threshold);
                return Ok(inventories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching low stock inventory", details = ex.Message });
            }
        }

        [HttpGet("out-of-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InventoryVM>>> GetOutOfStockInventory()
        {
            try
            {
                var inventories = await _inventoryService.GetOutOfStockInventoryAsync();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching out of stock inventory", details = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateInventory(int id, [FromBody] UpdateInventoryVM model)
        {
            try
            {
                var result = await _inventoryService.UpdateInventoryAsync(id, model);

                if (!result)
                {
                    return NotFound(new { error = $"Inventory with ID {id} not found" });
                }

                return Ok(new { message = $"Inventory with ID {id} updated successfully" });
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
                    new { error = "Error updating inventory", details = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{id}/adjust")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AdjustInventory(int id, [FromBody] AdjustInventoryVM model)
        {
            try
            {
                var result = await _inventoryService.AdjustInventoryAsync(id, model.QuantityChange);

                if (!result)
                {
                    return NotFound(new { error = $"Inventory with ID {id} not found" });
                }

                return Ok(new { message = $"Inventory with ID {id} adjusted by {model.QuantityChange} successfully" });
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
                    new { error = "Error adjusting inventory", details = ex.Message });
            }
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InventorySummaryVM>> GetInventorySummary()
        {
            try
            {
                var summary = await _inventoryService.GetInventorySummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error fetching inventory summary", details = ex.Message });
            }
        }
    }
}

/// <summary>
/// Helper DTO for adjusting inventory quantity
/// </summary>
public class AdjustInventoryVM
{
    public int QuantityChange { get; set; }
}
