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
            return Results.Ok(new Dictionary<string, string>
            {
                {"status", "Healthy" },
                {"timestamp", DateTime.UtcNow.ToString("")}, // ISO 8601 format for compatibility
                {"description", "API is running and healthy" }
            });
        }

        [HttpGet("fhir-service-health")]
        public async Task<IResult> GetAwsServiceHealth()
        {
            IServiceAvailabilityUtility serviceAvailabilityUtility = new ServiceAvailabilityUtility();
            List<string> serviceAvailable = await serviceAvailabilityUtility.ServiceAvailable();
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
                    {"status", "Availible" },
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