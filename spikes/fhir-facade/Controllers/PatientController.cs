using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Data;
using OneCDPFHIRFacade.Exceptions;
using OneCDPFHIRFacade.Handlers;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly LocalFileService _localFileService;
        private readonly S3FileService _s3FileService;
        public PatientController(LocalFileService localFileService, S3FileService s3FileService)
        {
            _localFileService = localFileService;
            _s3FileService = s3FileService;
            string patientId = "1";
            PatientData newPatient = new PatientData(patientId);
        }

        [HttpGet("{theId}")]
        public Patient GetPatient(string theId)
        {
            Id id = new Id(theId);
            Patient retVal = PatientDictData.PatientDictionary[id.Value];
            if (retVal == null)
            {
                throw new ResourceNotFoundException(id.Value);

            }
            return retVal;
        }
        // Use Dependency Injection to get services


        [HttpPost(Name = "PostPatient")]
        public async Task<IActionResult> Post([FromBody] Bundle bundle)
        {
            if (bundle == null)
            {
                return BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Patient is required."
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
