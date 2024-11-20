using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OneCDPOneCDPFHIRFacade.Handlers;


namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        [HttpPost(Name = "PostBundle")]
        public async Task<IResult> Post([FromBody] JObject requestBody)
        {
            string bundle;
            try
            {
                bundle = requestBody.ToString();
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
            var handler = new BundleHandler();

            // Use the handler to process the bundle and get the result
            var result = await handler.Post(bundle);

            // Return the result from the handler
            return result;
        }
    }
}
