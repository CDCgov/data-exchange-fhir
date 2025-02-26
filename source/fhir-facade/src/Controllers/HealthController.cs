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
            if (await serviceAvailabilityUtility.IsServiceAvailable())
            {
                return Results.Ok(new
                {
                    Static = "Availible",
                    timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                    description = "FHIR Facade services are available."
                });
            }
            else
            {
                return TypedResults.Problem("FHIR Facade services are not availible.", statusCode: (int)HttpStatusCode.ServiceUnavailable);

                //Results.Problem("FHIR Facade services are not availible.");
            }
        }

    }
}