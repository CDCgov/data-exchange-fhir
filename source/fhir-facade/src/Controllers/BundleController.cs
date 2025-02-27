using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Controllers
{
#if !runLocal
    [Authorize(Policy = "RequiredScope")]
#endif
    [ApiController]
    [Route("bundle")]
    public class BundleController : ControllerBase
    {
        //Create a cloud instance to add logs
        private readonly LoggingUtility _loggingUtility;
        bool runLocal = LocalFileStorageConfig.UseLocalDevFolder;
        public BundleController(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility;
        }

        private FileServiceFactory getFileServiceFactory()
        {
            return new FileServiceFactory(_loggingUtility);
        }

        [HttpPost]
        public async Task<IResult> Post()
        {

            FileServiceFactory fileServiceFactory = getFileServiceFactory();
            IFileService fileService = fileServiceFactory.CreateFileService(runLocal);

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
                string fileContent;

                //Read from File
                if (HttpContext.Request.HasFormContentType)
                {
                    // Ensure that the request is actually a file upload
                    if (!HttpContext.Request.ContentType!.Contains("multipart/form-data"))
                    {
                        logMessage = "Invalid content-type for form-data request.";
                        await _loggingUtility.Logging(logMessage);
                        return Results.BadRequest(new { error = "Invalid request", message = "Expected multipart/form-data but received a different content-type." });
                    }

                    var form = await HttpContext.Request.ReadFormAsync();
                    var file = form.Files.FirstOrDefault();

                    if (file == null || file.Length == 0)
                    {
                        logMessage = "No file uploaded or file is empty.";
                        await _loggingUtility.Logging(logMessage);
                        return Results.BadRequest(new { error = "Invalid request", message = "No file uploaded or file is empty." });
                    }
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin); // Reset position
                    fileContent = await new StreamReader(memoryStream).ReadToEndAsync();
                }
                //Read from body
                else if (HttpContext.Request.ContentType != null &&
                         HttpContext.Request.ContentType.StartsWith("application/json"))
                {
                    using var reader = new StreamReader(HttpContext.Request.Body);
                    fileContent = await reader.ReadToEndAsync();
                }
                else
                {
                    logMessage = "Unsupported content type.";
                    await _loggingUtility.Logging(logMessage);
                    return Results.BadRequest(new { error = "Invalid request", message = "Supported content types: application/json or multipart/form-data." });
                }

                // Parse JSON into a FHIR Bundle
                bundle = await parser.ParseAsync<Bundle>(fileContent);

                //Check that bundle profile matches user's scope
                if (!runLocal)
                {
                    BundleScopeValidation bundleScopeValidation = new BundleScopeValidation(bundle, _loggingUtility);
                    bool bundleScopeValid = await bundleScopeValidation.IsBundleProfileMatchScope();
                    if (bundleScopeValid)
                    {
                        Console.WriteLine("Bundle scope validated");
                    }
                    else
                    {
                        Console.WriteLine("Bundle scope not validated");
                        return Results.Forbid();
                    }
                }

                // Ensure bundle has a valid ID
                if (string.IsNullOrWhiteSpace(bundle.Id))
                {
                    logMessage = "Error: Invalid Payload. Message: Resource ID is required.";
                    await _loggingUtility.Logging(logMessage);
                    return Results.BadRequest(new { error = "Invalid payload", message = "Resource ID is required." });
                }

                logMessage = $"Received FHIR Bundle: Id={bundle.Id}";
                await _loggingUtility.Logging(logMessage);

                return await fileService.SaveResource( "Bundle", fileName, await bundle.ToJsonAsync());
            }
            catch (Exception ex)
            {
                logMessage = $"Failed to parse FHIR Resource: {ex.Message}";
                await _loggingUtility.Logging(logMessage);
                return Results.BadRequest(new { error = "Invalid payload", message = $"Failed to parse FHIR Resource: {ex.Message}" });
            }

        }
    }
}