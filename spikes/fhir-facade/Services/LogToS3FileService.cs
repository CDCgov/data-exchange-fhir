using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;

namespace OneCDPFHIRFacade.Services
{

    public interface ILogToS3FileService
    {
        Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName, string requestId);
    }
    public class LogToS3FileService : ILogToS3FileService
    {
        readonly List<string> resultList = new List<string>();
        public async Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName, string requestId)
        {
            string result = String.Join(",", resultList);

            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"Logs/{fileName}",
                ContentBody = result
            };

            // Attempt to save the resource to S3
            try
            {
                Console.WriteLine($"Writing logs to S3: fileName=Logs/{fileName}, bucket={bucketName}");
                await s3Client.PutObjectAsync(putRequest);
                return Results.Ok($"Logs saved successfully to S3 at Logs/{fileName}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving logs to S3: {ex.Message}");
            }
        }// .SaveResourceToS3
        public void JsonResult(string logs, string requestId)
        {
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = logs,
                Timestamp = DateTime.UtcNow,
            };
            var jsonLogMessage = JsonSerializer.Serialize(logMessage);

            resultList.Add(jsonLogMessage);
        }
    }
}