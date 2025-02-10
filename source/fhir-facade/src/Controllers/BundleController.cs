using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Controllers
{
    [Authorize(Policy = "RequiredScope")]
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        //Create a cloud instance to add logs
        private readonly LoggingUtility _loggingUtility;
        public BundleController(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility;
        }

        [HttpPost(Name = "PostBundle")]
        public async Task<IResult> Post()
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

            try
            {
                // Read the request body as a string
                var requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                // Parse JSON string to FHIR bundle object
                bundle = await parser.ParseAsync<Bundle>(requestBody.ToString());
            }
            catch (FormatException ex)
            {
                logMessage = $"Failed to parse FHIR Resource: {ex.Message}";
                await _loggingUtility.Logging(logMessage);
                await _loggingUtility.SaveLogS3(fileName);

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
                logMessage = "Error: Invalid Payload. Message: Resource ID is required.";
                await _loggingUtility.Logging(logMessage);
                await _loggingUtility.SaveLogS3(fileName);
                return Results.BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Resource ID is required."
                });
            }

            // TODO Check if the bundle Type is in line with the sender client scope 
            // TODO Example bundle type eICR, sender scope should system/eICR/bundle.c  -> how to insert eICR <-
            // TODO checkBundleTypeAgainstScope(bundle.Type, scopeClaim) 

            // Log details 
            logMessage = $"Received FHIR Bundle: Id={bundle.Id}";
            await _loggingUtility.Logging(logMessage);

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
                    logMessage = "S3 client and bucket are not configured.";
                    await _loggingUtility.Logging(logMessage);
                    await _loggingUtility.SaveLogS3(fileName);
                    return Results.Problem(logMessage);
                }

                return await s3FileService.SaveResourceToS3("Bundle", fileName, await bundle.ToJsonAsync());
            }// .else



        }

    }
}