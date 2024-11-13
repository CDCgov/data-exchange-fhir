using Amazon.S3;
using FirelyApiApp.Configs;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace FireFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : Controller
    {
        FileStorageConfig fileStorageConfig { get; set; }
        Patient patient { get; set; }
        FhirJsonParser fhirJsonParser { get; set; }
        LocalFileService localFileService { get; set; }
        S3FileService s3FileService { get; set; }

        public PatientController(FileStorageConfig fileStorageConfig, Patient patient, FhirJsonParser fhirJsonParser, LocalFileService localFileService, S3FileService s3FileService)
        {
            this.fileStorageConfig = fileStorageConfig;
            this.patient = patient;
            this.fhirJsonParser = fhirJsonParser;
            this.localFileService = localFileService;
            this.s3FileService = s3FileService;
        }

        [HttpPost(Name = "Patient")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult?> Index()
        {
            // Use FhirJsonParser to parse incoming JSON as FHIR Patient
            IAmazonS3? s3Client = null; // Declare s3Client as nullable
            String? s3BucketName = null;

            try
            {
                // Read the request body as a string
                var requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                // Parse JSON string to FHIR Patient object
                patient = fhirJsonParser.Parse<Patient>(requestBody);
            }
            catch (FormatException ex)
            {
                // Return 400 Bad Request if JSON is invalid
                return BadRequest(new
                {
                    error = "Invalid payload",
                    message = $"Failed to parse FHIR Resource: {ex.Message}"
                });
            }

            // Check if Patient ID is present
            if (string.IsNullOrWhiteSpace(patient.Id))
            {
                return BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Resource ID is required."
                });
            }

            // Log patient details to console
            Console.WriteLine($"Received FHIR Patient: Id={patient.Id}");

            // Generate a new UUID for the file name
            // Not using the patient.id: var filePath = Path.Combine(directoryPath, $"{patient.Id}.json");
            var fileName = $"{Guid.NewGuid()}.json";
            var resourceJson = patient.ToJson();

            if (fileStorageConfig.UseLocalDev)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                var localResult = await localFileService.SaveResourceLocally(fileStorageConfig.LocalDevFolder, "Patient", fileName, resourceJson);
                return (IActionResult)localResult;
            } // .if UseLocalDevFolder
            else
            {
                // #####################################################
                // Save the FHIR Resource to AWS S3
                // #####################################################
                if (s3Client == null || string.IsNullOrEmpty(s3BucketName))
                {
                    return Problem("S3 client and bucket are not configured.");
                }

                return (IActionResult?)await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Patient", fileName, resourceJson);
            }// .else
        }
    }
}
