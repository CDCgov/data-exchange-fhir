using Microsoft.AspNetCore.Mvc;

namespace FireFacade.Controllers
{
    // #####################################################
    // Define a health check endpoint at /health
    // #####################################################

    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet(Name = "Health")]
        public ActionResult Get()
        {
            return (ActionResult)Results.Json(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format for compatibility
                description = "API is running and healthy"
            });
        }
    }
}
