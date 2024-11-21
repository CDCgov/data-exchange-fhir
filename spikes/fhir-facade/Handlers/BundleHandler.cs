using Amazon.S3;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using OneCDPFHIRFacade.Configs;
using OneCDPFHIRFacade.Handlers;


namespace OneCDPOneCDPFHIRFacade.Handlers
{
    public class BundleHandler
    {
        LocalFileService localFileService = new LocalFileService();
        S3FileService s3FileService = new S3FileService();
        AWSHandler s3Handler = new AWSHandler();
        public async Task<IResult> Post(string json)
        {
            IAmazonS3? s3Client = s3Handler.AWSs3();
            String? s3BucketName = AWSConfig.BucketName;

            // Use FhirJsonParser to parse incoming JSON as FHIR bundle
            var parser = new FhirJsonParser();
            Bundle bundle;

            try
            {
                // Parse JSON string to FHIR bundle object
                bundle = parser.Parse<Bundle>(json);
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

            // Check if bundle ID is present
            if (string.IsNullOrWhiteSpace(bundle.Id))
            {
                return Results.BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Resource ID is required."
                });
            }

            // Log details to console
            Console.WriteLine($"Received FHIR Bundle: Id={bundle.Id}");

            // Generate a new UUID for the file name
            var fileName = $"{Guid.NewGuid()}.json";
            var resourceJson = bundle;

            if (FileStorageConfig.UseLocalDevFolder)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                return await localFileService.SaveResourceLocally(FileStorageConfig.LocalDevFolder, "Bundle", fileName, json);

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

                return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Bundle", fileName, json);
            }// .else
        }
    }
}
