using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ecommerce_application.SERVICES;
using ecommerce_application.MODELS.DB_MODELS;
using DAL.DTO;

namespace Ecommerce2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var allProducts = await _productService.GetAllProductsAsync();
                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load the products", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductVM model)
        {
            try
            {
                var productId = await _productService.AddProductAsync(model);
                return CreatedAtAction(nameof(GetProductById), new { productId },
                    new { id = productId, message = "Product added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't add the product", details = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpGet("active/list")]
        public async Task<IActionResult> GetActiveProducts()
        {
            try
            {
                var activeProducts = await _productService.GetActiveProductsAsync();
                return Ok(activeProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load the active products", details = ex.Message });
            }
        }

        [HttpGet("id/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found");
                }
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load the product", details = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                if (products == null)
                {
                    return NotFound($"No products found for category {categoryId}");
                }
                return Ok(products);
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
                return BadRequest(new { error = "We couldn't load products by category", details = ex.Message });
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<IActionResult> SearchProducts(string searchTerm)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(searchTerm);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't search products", details = ex.Message });
            }
        }

        [HttpPut("id/{productId}")]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductVM model)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(productId, model);

                if (!result)
                {
                    return NotFound(new { error = $"Product with ID {productId} not found" });
                }

                return Ok(new { message = $"Product with ID {productId} updated successfully" });
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
                return BadRequest(new { error = "We couldn't update the product", details = ex.Message });
            }
        }

        [HttpDelete("id/{productId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(productId);
                if (!result)
                {
                    return NotFound(new { error = $"Product with ID {productId} not found" });
                }
                return Ok(new { message = $"Product with ID {productId} deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't delete the product", details = ex.Message });
            }
        }

        [HttpPatch("id/{productId}/deactivate")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeactivateProduct(int productId)
        {
            try
            {
                var result = await _productService.DeactivateProductAsync(productId);
                if (!result)
                {
                    return NotFound(new { error = $"Product with ID {productId} not found" });
                }
                return Ok(new { message = $"Product with ID {productId} deactivated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't deactivate the product", details = ex.Message });
            }
        }

        [HttpPatch("id/{productId}/activate")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ActivateProduct(int productId)
        {
            try
            {
                var result = await _productService.ActivateProductAsync(productId);
                if (!result)
                {
                    return NotFound(new { error = $"Product with ID {productId} not found" });
                }
                return Ok(new { message = $"Product with ID {productId} activated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't activate the product", details = ex.Message });
            }
        }

        [HttpGet("{categoryId}/products-with-count")]
        public async Task<IActionResult> GetProductsByCategoryWithCount(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryWithCountAsync(categoryId);
                return Ok(products);
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
                return BadRequest(new { error = "We couldn't load products with count", details = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetProductStatistics()
        {
            try
            {
                var statistics = await _productService.GetProductStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "We couldn't load product statistics", details = ex.Message });
            }
        }
    }
}

/// <summary>
/// Helper DTO for updating stock quantity
/// </summary>
public class UpdateStockVM
{
    public int NewQuantity { get; set; }
}