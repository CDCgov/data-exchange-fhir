using Amazon.S3;
using fhirfacade.Configs;
using Hl7.Fhir.Model;
using NuGet.Protocol;


namespace fhirfacade.Handlers
{
    public class BundleHandler
    {
        LocalFileService localFileService;

        S3FileService s3FileService;

        public BundleHandler(LocalFileService localFileService, S3FileService s3FileService)
        {
            this.localFileService = localFileService;
            this.s3FileService = s3FileService;
        }
        public async Task<IResult> Post(Bundle bundle)
        {
            IAmazonS3? s3Client = null; // Declare s3Client as nullable
            String? s3BucketName = null;

            // Check if Patient ID is present
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
            var resourceJson = bundle.ToJson();

            if (FileStorageConfig.UseLocalDevFolder)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                return await localFileService.SaveResourceLocally(FileStorageConfig.LocalDevFolder, "Bundle", fileName, resourceJson);
            }
            else
            {
                // #####################################################
                // Save the FHIR Resource to AWS S3
                // #####################################################
                if (s3Client == null || string.IsNullOrEmpty(s3BucketName))
                {
                    return Results.Problem("S3 client and bucket are not configured.");
                }

                return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Bundle", fileName, resourceJson);
            }
        }
    }
}
