using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Controllers
{
#if !runLocal
    [Authorize(Policy = "RequiredScope")]
#endif
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        //Create a cloud instance to add logs
        private readonly LoggingUtility _loggingUtility;
        bool runLocal = LocalFileStorageConfig.UseLocalDevFolder;
        public BundleController(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility;
        }

        [HttpPost(Name = "PostBundle")]
        [RequestSizeLimit(300 * 1024 * 1024)] //300MB limit
        public async Task<IResult> Post([FromForm] BundleUploadRequest request)
        {
            LocalFileService localFileService = new LocalFileService(_loggingUtility);
            S3FileService s3FileService = new S3FileService(_loggingUtility);

            // Use FhirJsonParser to parse incoming JSON as FHIR bundle
            var parser = new FhirJsonParser();
            Bundle bundle;

            // Generate a new UUID for the file name
            var randomID = $"{Guid.NewGuid()}";
            string fileName = $"{randomID}.json";

            //Log starts
            string logMessage = "Bundle request has started.";
            await _loggingUtility.Logging(logMessage);
            string fileContent = string.Empty;

            try
            {
                // Determine whether JSON is coming from a file upload or direct request body
                if (request.File != null && request.File.Length > 0)
                {
                    using var reader = new StreamReader(request.File.OpenReadStream());
                    fileContent = await reader.ReadToEndAsync();
                }
                else if (!string.IsNullOrWhiteSpace(request.Json))
                {
                    fileContent = request.Json;
                }
                else
                {
                    logMessage = "Invalid request: No valid JSON payload provided.";
                    await _loggingUtility.Logging(logMessage);
                    return Results.BadRequest(new { error = "Invalid request", message = "Expected JSON data or a file upload." });
                }

                // Parse JSON into a FHIR Bundle
                bundle = await parser.ParseAsync<Bundle>(fileContent);

                //Check that bundle profile matches user's scope
                //if (!runLocal)
                //{
                //    BundleScopeValidation bundleScopeValidation = new BundleScopeValidation(bundle, _loggingUtility);
                //    bool bundleScopeValid = await bundleScopeValidation.IsBundleProfileMatchScope();
                //    if (!bundleScopeValid)
                //    {
                //        logMessage = "Bundle scope not validated";
                //        await _loggingUtility.Logging(logMessage);
                //        return Results.Forbid();
                //    }
                //}

                // Ensure bundle has a valid ID
                if (string.IsNullOrWhiteSpace(bundle.Id))
                {
                    logMessage = "Error: Invalid Payload. Message: Resource ID is required.";
                    await _loggingUtility.Logging(logMessage);
                    return Results.BadRequest(new { error = "Invalid payload", message = "Resource ID is required." });
                }

                logMessage = $"Received FHIR Bundle: Id={bundle.Id}";
                await _loggingUtility.Logging(logMessage);

                // Save based on environment (local or cloud)
                if (runLocal)
                {
                    return await localFileService.SaveResourceLocally(
                        LocalFileStorageConfig.LocalDevFolder!,
                        "Bundle",
                        fileName,
                        await bundle.ToJsonAsync()
                    );
                }
                else
                {
                    if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
                    {
                        logMessage = "S3 client and bucket are not configured.";
                        await _loggingUtility.Logging(logMessage);
                        return Results.Problem(logMessage);
                    }

                    return await s3FileService.SaveResourceToS3("Bundle", fileName, await bundle.ToJsonAsync());
                }
            }
            catch (Exception ex)
            {
                logMessage = $"Failed to parse FHIR Resource: {ex.Message}";
                await _loggingUtility.Logging(logMessage);
                return Results.BadRequest(new { error = "Invalid payload", message = $"Failed to parse FHIR Resource: {ex.Message}" });
            }

        }
    }
    public class BundleUploadRequest
    {
        [FromForm(Name = "json")]
        public string? Json { get; set; }

        [FromForm(Name = "file")]
        public IFormFile? File { get; set; }
    }
}