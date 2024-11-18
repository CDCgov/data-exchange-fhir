using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
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
        private Dictionary<String, Patient> myPatients = new Dictionary<String, Patient>();
        public PatientController(LocalFileService localFileService, S3FileService s3FileService)
        {
            _localFileService = localFileService;
            _s3FileService = s3FileService;

            Patient pat1 = new Patient();
            pat1.Id = "1";
            Identifier identifier = new Identifier
            {
                System = Environment.GetEnvironmentVariable("PATIENT_IDENTIFIER_SYSTEM"),
                Value = Environment.GetEnvironmentVariable("PATIENT_IDENTIFIER_VALUE")
            };
            pat1.Identifier.Add(identifier);
            HumanName patient1 = new HumanName()
            {
                Family = "Simpson",
                Given = ["Homer", " J."]
            };
            pat1.Name.Add(patient1);
            myPatients.Add("1", pat1);
        }

        [HttpGet("{theId}")]
        public Patient GetPatient(string theId)
        {
            Id id = new Id(theId);
            Patient retVal = myPatients[id.Value];
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
