using Amazon.S3;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using OneCDPFHIRFacade.Configs;

namespace OneCDPFHIRFacade.Handlers
{
    public class PatientHandler
    {
        LocalFileService localFileService = new LocalFileService();
        S3FileService s3FileService = new S3FileService();

        public async Task<IResult> CreatePatient(JsonType json)
        {
            IAmazonS3? s3Client = null; // Declare s3Client as nullable
            String? s3BucketName = null;

            // Use FhirJsonParser to parse incoming JSON as FHIR patient
            Patient patient;
            var parser = new FhirJsonParser();
            try
            {
                patient = parser.Parse<Patient>(json);

                if (patient == null)
                {
                    return Results.BadRequest(new { error = "Invalid FHIR resource." });
                }
                // Read the request body as a string

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

            // Check if Patient ID is present
            if (string.IsNullOrWhiteSpace(patient.Id))
            {
                return Results.BadRequest(new
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

            if (FileStorageConfig.UseLocalDevFolder)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                return await localFileService.SaveResourceLocally(FileStorageConfig.LocalDevFolder, "Patient", fileName, resourceJson);

            } // .if UseLocalDevFolder
            else
            {
                // #####################################################
                // Save the FHIR Resource to AWS S3
                // #####################################################
                if (s3Client == null || string.IsNullOrEmpty(s3BucketName))
                {
                    return Results.Problem("S3 client and bucket are not configured.");
                }

                return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Patient", fileName, resourceJson);
            }// .else
        }
    }
}
