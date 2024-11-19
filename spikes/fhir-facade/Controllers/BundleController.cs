using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using OneCDPOneCDPFHIRFacade.Handlers;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {

        [HttpPost(Name = "PostBundle")]
        public async Task<IActionResult> Post([FromBody] Bundle bundle)
        {
            if (HttpContext == null)
            {
                return BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Bundle is required."
                });
            }

            // Create the handler with the injected dependencies
            var handler = new BundleHandler();

            // Use the handler to process the bundle and get the result
            var result = await handler.Post(bundle);

            // Return the result from the handler
            return (IActionResult)result;
        }
    }
}
