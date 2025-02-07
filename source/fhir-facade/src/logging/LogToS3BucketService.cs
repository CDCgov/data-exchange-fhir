using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OneCDP.Logging
{
    public interface ILogToS3BucketService
    {
        Task<IActionResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName);
        void JsonResult(object logs);
    }

    public class LogToS3BucketService : ILogToS3BucketService
    {
        public List<object> _resultList = new();

        public async Task<IActionResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string fileName)
        {
            var jsonResult = JsonSerializer.Serialize(_resultList);
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"Logs/{fileName}",
                ContentBody = jsonResult
            };

            try
            {
                Console.WriteLine($"Writing logs to S3: fileName=Logs/{fileName}, bucket={bucketName}");
                await s3Client.PutObjectAsync(putRequest);
                _resultList = [];
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

        public void JsonResult(object logs)
        {
            _resultList.Add(logs);
        }
    }

}