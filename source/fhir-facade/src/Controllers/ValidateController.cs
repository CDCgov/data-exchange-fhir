using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Controllers
{
#if !runLocal
    [Authorize(Policy = "RequiredScope")]
#endif
    [ApiController]
    [Route("validate")]
    public class ValidateController : Controller
    {
        [HttpPost]
        public async Task<IResult> Post()
        {
            var parser = new FhirJsonParser();
            Bundle bundle;
            string message;

            try
            {
                string fileContent;
                //Read from File
                if (HttpContext.Request.HasFormContentType)
                {
                    // Ensure that the request is actually a file upload
                    if (!HttpContext.Request.ContentType!.Contains("multipart/form-data"))
                    {
                        message = "Invalid content-type for form-data request.";
                        Console.WriteLine(message);
                        return Results.BadRequest(new Dictionary<string, string>
                        {
                            { "error" , "Invalid request" },
                            { "message" , "Expected multipart/form-data but received a different content-type." }
                        });
                    }

                    var form = await HttpContext.Request.ReadFormAsync();
                    var file = form.Files.FirstOrDefault();

                    if (file == null || file.Length == 0)
                    {
                        message = "No file uploaded or file is empty.";
                        Console.WriteLine(message);
                        return Results.BadRequest(new Dictionary<string, string>
                        {
                            { "error", "Invalid request"},
                            { "message", "No file uploaded or file is empty." }
                        });
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
                    message = "Unsupported content type.";
                    Console.WriteLine(message);
                    return Results.BadRequest(new Dictionary<string, string>
                    {
                        { "error", "Invalid request"},
                        { "message", "Supported content types: application/json or multipart/form-data." }
                    });
                }
                try
                {
                    // Parse JSON into a FHIR Bundle
                    ValidationUtility validationUtility = new ValidationUtility();

                    bundle = await parser.ParseAsync<Bundle>(fileContent);
                    string validBundle = validationUtility.ValidateBundle(bundle);
                    Console.WriteLine(validBundle);
                    if (validBundle.Contains("true"))
                        return Results.Ok("");
                    else
                    {
                        return Results.Problem(validBundle);
                    }
                }
                catch
                {
                    return Results.BadRequest(new Dictionary<string, string>
                    {
                        { "error", "Invalid request"},
                        { "message", "Unable to parse request." }
                    });
                }
            }
            catch (Exception ex)
            {
                message = $"Failed to parse FHIR Resource: {ex.Message}";
                Console.WriteLine(message);
                return Results.BadRequest(new { error = "Invalid payload", message = $"Failed to parse FHIR Resource: {ex.Message}" });
            }

        }
    }
}
