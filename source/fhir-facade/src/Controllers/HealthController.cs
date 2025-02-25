using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Controllers
{
    // #####################################################
    // Define a health check endpoint at /health
    // #####################################################

    [ApiController]
    [Route("[Controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet(Name = "Health")]
        public IResult GetHealth()
        {
            return Results.Json(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                description = "API is running and healthy"
            });
        }

        [HttpGet("LogAvailibility")]
        public async Task<IResult> GetLogAvailibility()
        {
            ServiceAvailibilityUtility serviceAvailibilityUtility = new ServiceAvailibilityUtility();
            if (await serviceAvailibilityUtility.IsLogGroupAvailable())
            {
                return Results.Ok(new
                {
                    Static = "Availible",
                    timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                    description = "Log Group is availible."
                });
            }
            else
            {
                return Results.Problem("Log Group is not availible.");
            }
        }
        [HttpGet("S3Availibility")]
        public async Task<IResult> GetS3Availibility()
        {
            ServiceAvailibilityUtility serviceAvailibilityUtility = new ServiceAvailibilityUtility();
            if (await serviceAvailibilityUtility.IsLogGroupAvailable())
            {
                return Results.Ok(new
                {
                    Static = "Availible",
                    timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                    description = $"S3 Bucket {AwsConfig.BucketName} is availible."
                });
            }
            else
            {
                return Results.Problem("S3 client and bucket are not configured.");
            }
        }

    }
}