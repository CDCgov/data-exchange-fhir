using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using System.Text.RegularExpressions;

namespace OneCDPFHIRFacade.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        //Create a cloud instance to add logs
        CloudWatchLoggerService logEntry = new CloudWatchLoggerService();

        [HttpPost(Name = "PostBundle")]
        public async Task<IResult> Post()
        {
            LocalFileService localFileService = new LocalFileService();
            S3FileService s3FileService = new S3FileService();

            var requestId = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

            // Use FhirJsonParser to parse incoming JSON as FHIR bundle
            var parser = new FhirJsonParser();
            Bundle bundle;

            //Log starts
            await logEntry.AppendLogAsync($"requestID: {requestId}. Bundle request has started.");

            try
            {
                // Read the request body as a string
                var requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                // Parse JSON string to FHIR bundle object
                bundle = await parser.ParseAsync<Bundle>(requestBody.ToString());
            }
            catch (FormatException ex)
            {
                await logEntry.AppendLogAsync($"requestID: {requestId}. Failed to parse FHIR Resource: {ex.Message}");
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
                await logEntry.AppendLogAsync($"requestID: {requestId}. Error: Invalid Payload. Message: Resource ID is required.");
                return Results.BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Resource ID is required."
                });
            }

            // Log details to console
            Console.WriteLine($"Received FHIR Bundle: Id={bundle.Id}");
            await logEntry.AppendLogAsync($"requestID: {requestId}. Received FHIR Bundle: Id={bundle.Id}");

            // Generate a new UUID for the file name
            var fileName = $"{Guid.NewGuid()}.json";

            if (LocalFileStorageConfig.UseLocalDevFolder)
            {
                // #####################################################
                // Save the FHIR Resource Locally
                // #####################################################
                return await localFileService.SaveResourceLocally(LocalFileStorageConfig.LocalDevFolder!, "Bundle", fileName, await bundle.ToJsonAsync());

            } // .if UseLocalDevFolder
            else
            {
                // #####################################################
                // Save the FHIR Resource to AWS S3
                // #####################################################
                if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
                {
                    await logEntry.AppendLogAsync($"requestID: {requestId}. S3 client and bucket are not configured.");
                    return Results.Problem("S3 client and bucket are not configured.");
                }
                return await s3FileService.SaveResourceToS3(AwsConfig.S3Client, AwsConfig.BucketName, "Bundle", fileName, await bundle.ToJsonAsync(), logEntry, requestId);
            }// .else
        }

    }
}