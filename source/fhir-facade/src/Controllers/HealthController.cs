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
        private readonly IServiceAvailabilityUtility _serviceAvailabilityUtility;

        // Inject IServiceAvailabilityUtility via constructor
        public HealthController(IServiceAvailabilityUtility serviceAvailabilityUtility)
        {
            _serviceAvailabilityUtility = serviceAvailabilityUtility;
        }

        [HttpGet("system-health")]
        public IResult GetHealth()
        {
            return Results.Json(new Dictionary<string, string>
            {
                {"status", "Healthy"},
                {"timestamp", DateTime.UtcNow.ToString("")}, // ISO 8601 format for compatibility
                {"description", "API is running and healthy" }
            });
        }

        [HttpGet("fhir-service-health")]
        public async Task<IResult> GetAwsServiceHealth()
        {
            List<string> serviceAvailable = await _serviceAvailabilityUtility.ServiceAvailable();

            string message = "";
            foreach (string item in serviceAvailable)
            {
                if (message.Length > 0)
                    message += " ";
                message += item;
            }
            if (!serviceAvailable.Any(s => s.Contains("unavailable")))
            {
                return Results.Ok(new Dictionary<string, string>
                {
                    {"status", "Available" },
                    {"timestamp", DateTime.UtcNow.ToString("")}, // ISO 8601 format for compatibility
                    {"description", message }
                });
            }
            else
            {
                return TypedResults.Problem(message, statusCode: (int)HttpStatusCode.ServiceUnavailable);
            }
        }

    }
}