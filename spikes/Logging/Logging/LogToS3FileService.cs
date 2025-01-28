using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OneCDP.Logging
{
    public interface ILogToS3BucketService
    {
        Task<IActionResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName, string requestId);
        void JsonResult(string logs, string requestId);
    }

    public class LogToS3BucketService : ILogToS3BucketService
    {
        private readonly List<string> _resultList = new();

        public async Task<IActionResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName, string requestId)
        {
            string result = string.Join(",", _resultList);

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"Logs/{fileName}",
                ContentBody = result
            };

            try
            {
                Console.WriteLine($"Writing logs to S3: fileName=Logs/{fileName}, bucket={bucketName}");
                await s3Client.PutObjectAsync(putRequest);
                return new OkObjectResult($"Logs saved successfully to S3 at Logs/{fileName}");
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Error saving logs to S3: {ex.Message}")
                {
                    StatusCode = 500
                };
            }
        }

        public void JsonResult(string logs, string requestId)
        {
            var logMessage = new
            {
                RequestID = requestId,
                Message = logs,
                Timestamp = DateTime.UtcNow,
            };
            var jsonLogMessage = JsonSerializer.Serialize(logMessage);

            _resultList.Add(jsonLogMessage);
        }
    }

}