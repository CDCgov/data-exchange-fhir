using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Utilities;
using System.Net;

namespace OneCDPFHIRFacade.Controllers
{
    // #####################################################
    // Define a health check endpoint at /health
    // #####################################################

    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("system-health")]
        public IResult GetHealth()
        {
            return Results.Json(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                description = "API is running and healthy"
            });
        }

        [HttpGet("fhir-service-health")]
        public async Task<IResult> GetAwsServiceHealth()
        {
            ServiceAvailabilityUtility serviceAvailabilityUtility = new ServiceAvailabilityUtility();
            List<string> serviceAvailable = await serviceAvailabilityUtility.ServiceAvailable();
            if (!serviceAvailable.Any(s => s.Contains("unavailable")))
            {
                return Results.Ok(new
                {
                    Static = "Availible",
                    timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                    description = serviceAvailable
                });
            }
            else
            {
                string message = "";
                foreach (string item in serviceAvailable)
                {
                    if (message.Length > 0)
                        message += " ";
                    message += item;
                }
                return TypedResults.Problem(message, statusCode: (int)HttpStatusCode.ServiceUnavailable);
            }
        }

    }
}