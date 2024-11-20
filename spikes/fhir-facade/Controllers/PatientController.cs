using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OneCDPFHIRFacade.Handlers;

namespace OneCDPOneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {

        [HttpPost]
        public async Task<IResult> Post([FromBody] JObject requestBody)
        {
            string patient;
            try
            {
                patient = requestBody.ToString();
            }
            catch (FormatException ex)
            {
                // Return 400 Bad Request if JSON is invalid
                return Results.BadRequest(new
                {
                    error = "Invalid payload",
                    message = $"Failed to parse FHIR Resource: {ex.Message}"
                });
            }
            // Create the handler with the injected dependencies
            PatientHandler handler = new PatientHandler();

            // Return the result from the handler
            return await handler.CreatePatient(patient);

        }
    }
}
