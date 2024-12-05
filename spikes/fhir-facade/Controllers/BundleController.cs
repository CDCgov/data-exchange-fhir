using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        private readonly ILogger<BundleController> _logger;

        [HttpPost(Name = "PostBundle")]
        public async Task<IResult> Post()
        {
            LocalFileService localFileService = new LocalFileService();
            S3FileService s3FileService = new S3FileService();

            // Use FhirJsonParser to parse incoming JSON as FHIR bundle
            var parser = new FhirJsonParser();
            Bundle bundle;

            ////AWS CloudWatch logs instance
            //var credentials = new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey);

            //var config = new AmazonCloudWatchLogsConfig
            //{
            //    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(AwsConfig.Region)
            //};

            //var logClient = new AmazonCloudWatchLogsClient(credentials, config);
            //var logGroupName = "/aws/bundle-logs/";
            //var logStreamName = $"{DateTime.UtcNow.ToString("yyyyMMdd")}";

            ////Create a new log group
            ////await logClient.CreateLogGroupAsync(new CreateLogGroupRequest(logGroupName));
            ////await logClient.CreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));
            ////await logClient.PutLogEventsAsync(new PutLogEventsRequest()
            ////{
            ////    LogGroupName = logGroupName,
            ////    LogStreamName = logStreamName,
            ////    LogEvents = new List<InputLogEvent>()
            ////    {
            ////        new InputLogEvent() {Message = "Get bundle request", Timestamp = DateTime.UtcNow}
            ////    }
            ////});
            //var describeLogStreamsRequest = new DescribeLogStreamsRequest
            //{
            //    LogGroupName = logGroupName,
            //    LogStreamNamePrefix = logStreamName
            //};
            //var response = await logClient.DescribeLogStreamsAsync(describeLogStreamsRequest);
            ////Check if Logstream is there already
            //if (!response.LogStreams.Any(ls => ls.LogStreamName == logStreamName))
            //{
            //    //Add to a logs group
            //    await logClient.CreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));
            //}

            ////Write to log stream
            //var addLogEventsRequest = new PutLogEventsRequest
            //{
            //    LogGroupName = "/aws/bundle-logs/",
            //    LogStreamName = $"{DateTime.UtcNow.ToString("yyyyMMdd")}",
            //    LogEvents = new List<InputLogEvent>
            //    {
            //        new InputLogEvent
            //        {
            //            Message = "A new Bundle has been sent.",
            //            Timestamp = DateTime.UtcNow
            //        }
            //    }
            //};

            //await logClient.PutLogEventsAsync(addLogEventsRequest);

            //await logClient.PutLogEventsAsync(addLogEventsRequest);
            CloudWatchLoggerService logEntry = new CloudWatchLoggerService();
            await logEntry.AppendLogAsync("myMessage");

            try
            {
                // Read the request body as a string
                var requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                // Parse JSON string to FHIR bundle object
                bundle = await parser.ParseAsync<Bundle>(requestBody.ToString());
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
                    return Results.Problem("S3 client and bucket are not configured.");
                }

                return await s3FileService.SaveResourceToS3(AwsConfig.S3Client, AwsConfig.BucketName, "Bundle", fileName, await bundle.ToJsonAsync());
            }// .else
        }

    }
}