// S3FileService.cs
// #####################################################
// SaveResourceToS3
// #####################################################

using Amazon.S3.Model;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
using System.Text.Json;

namespace OneCDPFHIRFacade.Services
{
    public interface IS3FileService
    {
        Task<IResult> SaveResourceToS3(string folderName, string fileName, string content, string requestId);
    }
    public class S3FileService : IS3FileService
    {
        private readonly LoggingUtility LoggingUtility;

        public S3FileService(LoggingUtility loggingUtility)
        {
            this.LoggingUtility = loggingUtility;
        }

        public async Task<IResult> SaveOpenTelemetryToS3(string folderName, string fileName, string content, string requestId)
        {
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = AwsConfig.BucketName,
                Key = $"{folderName}/{fileName}",
                ContentBody = content
            };
            string jsonString = JsonSerializer.Serialize(putRequest);

            try
            {
                await LoggingUtility.Logging(jsonString, requestId);
                await AwsConfig.S3Client!.PutObjectAsync(putRequest);
                return Results.Ok($"Telemtry saved successfully to S3 at {folderName}");
            }
            catch (Exception ex)
            {
                string logMessage = $"Error saving resource to S3: {ex.Message}";
                await LoggingUtility.Logging(logMessage, requestId);
                await LoggingUtility.SaveLogS3(fileName);
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }
        public async Task<IResult> SaveResourceToS3(string folderName, string fileName, string content, string requestId)
        {
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = AwsConfig.BucketName,
                Key = $"{folderName}/{fileName}",
                ContentBody = content
            };

            // Attempt to save the resource to S3
            try
            {
                string logMessage = $"End writing to S3: fileName={fileName}, bucket={AwsConfig.BucketName}, requestId={requestId}";

                await LoggingUtility.Logging(logMessage, requestId);
                Console.WriteLine(logMessage);

                var response = await AwsConfig.S3Client!.PutObjectAsync(putRequest);

                string logString = $"End write to S3: fileName={fileName}, response={response.HttpStatusCode}, requestId: {requestId}";
                await LoggingUtility.Logging(logString, requestId);

                Console.WriteLine(logString);

                await LoggingUtility.SaveLogS3(fileName);
                return Results.Ok($"Resource saved successfully to S3 at {folderName}/{fileName}");
            }
            catch (Exception ex)
            {
                string logMessage = $"Error saving resource to S3: {ex.Message}";
                await LoggingUtility.Logging(logMessage, requestId);
                await LoggingUtility.SaveLogS3(fileName);
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }// .SaveResourceToS3

    }// .S3FileService

} // .namespace