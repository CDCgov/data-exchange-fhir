using fhirfacade.Handlers;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace fhirfacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        private readonly LocalFileService _localFileService;
        private readonly S3FileService _s3FileService;

        // Use Dependency Injection to get services
        public BundleController(LocalFileService localFileService, S3FileService s3FileService)
        {
            _localFileService = localFileService;
            _s3FileService = s3FileService;
        }

        [HttpPost(Name = "PostBundle")]
        public async Task<IActionResult> Post([FromBody] Bundle bundle)
        {
            if (bundle == null)
            {
                return BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Bundle is required."
                });
            }

            // Create the handler with the injected dependencies
            var handler = new BundleHandler(_localFileService, _s3FileService);

            // Use the handler to process the bundle and get the result
            var result = await handler.Post(bundle);

            // Return the result from the handler
            return (IActionResult)result;
        }
    }
}
