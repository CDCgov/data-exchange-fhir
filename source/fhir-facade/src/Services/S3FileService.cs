// S3FileService.cs
// #####################################################
// SaveResourceToS3
// #####################################################

using Amazon.S3.Model;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Services
{
    public interface IS3FileService
    {
        Task<IResult> SaveResourceToS3(string folderName, string fileName, string content, LoggerService logEntry, string requestId);
        // TODO: should this be IActionResult? vs. IResult 
    }
    public class S3FileService : IS3FileService
    {
        public LoggingUtility LoggingUtility;

        public S3FileService(LoggingUtility loggingUtility)
        {
            this.LoggingUtility = loggingUtility;
        }

        public async Task<IResult> SaveOpenTelemetryToS3(string keyPrefix, string fileName, string resourceJson, LoggerService logEntry, string requestId)
        {
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = AwsConfig.BucketName,
                Key = $"{keyPrefix}/{fileName}",
                ContentBody = resourceJson
            };

            try
            {
                var response = await AwsConfig.S3Client!.PutObjectAsync(putRequest);
                LoggingUtility.Logging(logEntry.ToString()!, requestId);
                return Results.Ok($"Telemtry Saved to S3 bucket {fileName}");
            }
            catch (Exception ex)
            {
                string logMessage = $"Error saving resource to S3: {ex.Message}";
                LoggingUtility.Logging(logMessage, requestId);
                LoggingUtility.SaveLogS3(requestId);
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }
        public async Task<IResult> SaveResourceToS3(string keyPrefix, string fileName, string resourceJson, LoggerService logEntry, string requestId)
        {
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = AwsConfig.BucketName,
                Key = $"{keyPrefix}/{fileName}",
                ContentBody = resourceJson
            };

            // Attempt to save the resource to S3
            try
            {
                string logMessage = $"End writing to S3: fileName={fileName}, bucket={AwsConfig.BucketName}, requestId={requestId}";

                LoggingUtility.Logging(logMessage, requestId);
                Console.WriteLine(logMessage);

                var response = await AwsConfig.S3Client!.PutObjectAsync(putRequest);

                string logString = $"End write to S3: fileName={fileName}, response={response.HttpStatusCode}, requestId: {requestId}";
                LoggingUtility.Logging(logString, requestId);

                Console.WriteLine(logString);

                LoggingUtility.SaveLogS3(requestId);
                return Results.Ok($"Resource saved successfully to S3 at {keyPrefix}/{fileName}");
            }
            catch (Exception ex)
            {
                string logMessage = $"Error saving resource to S3: {ex.Message}";
                LoggingUtility.Logging(logMessage, requestId);
                LoggingUtility.SaveLogS3(requestId);
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }// .SaveResourceToS3

    }// .S3FileService

} // .namespace
