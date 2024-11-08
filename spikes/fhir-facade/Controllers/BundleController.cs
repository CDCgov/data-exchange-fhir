using Amazon.S3;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace FirelyApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {

        [HttpPost(Name = "PostBundle")]
        public async Task<IResult> Post([FromBody] HttpContext httpContext)
        {

            var parser = new FhirJsonParser();
            var UseLocalDevFolder = true;
            //builder.Configuration.GetValue<bool>("UseLocalDevFolder");
            var UseAWSS3 = !UseLocalDevFolder;
            Bundle bundle;
            var lfs = new LocalFileService();
            var localReceivedFolder = "C:\\Users\\AC56\\Desktop\\Dev";
            var s3FileService = new S3FileService();
            IAmazonS3? s3Client = null; // Declare s3Client as nullable
            String? s3BucketName = null;

            try
            {
                // Read the request body as a string
                var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                // Parse JSON string to FHIR Patient object
                bundle = parser.Parse<Bundle>(requestBody);
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

            if (UseLocalDevFolder)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                return await lfs.SaveResourceLocally(localReceivedFolder, "Bundle", fileName, resourceJson);

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

                return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Bundle", fileName, resourceJson);
            }// .else
        }
    }
}
