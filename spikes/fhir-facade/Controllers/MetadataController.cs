using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Handlers;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : Controller
    {
        /**
         * Receive a Patient bundle to process. This method prints the Patient bundle to
         * console and also sends to S3 bucket.
         * 
         * @param thePatient
         * @return MethodOutcome
         */



        [HttpGet]
        public IResult Index()
        {
            // Create a CapabilityStatement object with the server's metadata
            CapabilityStatement capabilityStatement = CapabilityStatements.CreateCapabilityStatement();

            // Serialize the CapabilityStatement to JSON
            var json = new Hl7.Fhir.Serialization.FhirJsonSerializer().SerializeToString(capabilityStatement);
            return Results.Json(capabilityStatement, contentType: "application/fhir+json");

            // Return the JSON response
            //context.Response.ContentType = "application/fhir+json";
            //await context.Response.WriteAsync(json);

        }
    }
}
