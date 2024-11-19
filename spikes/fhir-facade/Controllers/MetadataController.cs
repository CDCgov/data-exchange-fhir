using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : Controller
    {
        [HttpGet]
        public IResult Index()
        {
            // Create a CapabilityStatement object with the server's metadata
            var capabilityStatement = new CapabilityStatement
            {
                // Populate the CapabilityStatement with relevant details
                FhirVersion = FHIRVersion.N4_0_1, // Specify the FHIR version
                Description = new Markdown("This is a sample FHIR server exposing the $metadata operation."),
                Software = new CapabilityStatement.SoftwareComponent
                {
                    Name = "Sample FHIR Server",
                    Version = "1.0.0"
                },
                Implementation = new CapabilityStatement.ImplementationComponent
                {
                    Description = "Sample implementation of a FHIR server"
                }
            };

            // Serialize the CapabilityStatement to JSON
            var json = new Hl7.Fhir.Serialization.FhirJsonSerializer().SerializeToString(capabilityStatement);
            return Results.Json(capabilityStatement, contentType: "application/fhir+json");

            // Return the JSON response
            //context.Response.ContentType = "application/fhir+json";
            //await context.Response.WriteAsync(json);

        }
    }
}
