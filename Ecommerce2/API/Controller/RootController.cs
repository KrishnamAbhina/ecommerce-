using Microsoft.AspNetCore.Mvc;

namespace Ecommerce2.API.Controller
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// API root endpoint
        /// </summary>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = false)]
        public ActionResult<object> ApiRoot()
        {
            return Ok(new
            {
                message = "E-commerce API",
                version = "1.0",
                endpoints = new
                {
                    swagger = "/swagger/ui",
                    products = "/api/product",
                    documentation = "/swagger"
                }
            });
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
        }
    }
}