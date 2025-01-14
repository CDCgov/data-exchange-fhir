using Amazon.S3;
using Amazon.S3.Model;

namespace OneCDPFHIRFacade.Services
{

    public interface ILogToS3FileService
    {
        Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string folderName, string fileName, string[] content, string requestId);
    }
    public class LogToS3FileService : ILogToS3FileService
    {
        public async Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string s3BucketName, string folderName, string fileName, string[] logs, string requestId)
        {
            string result = String.Join(",", logs);
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = s3BucketName,
                Key = $"Logs/{folderName}/{fileName}",
                ContentBody = result
            };

            // Attempt to save the resource to S3
            try
            {
                Console.WriteLine($"Writing logs to S3: fileName={fileName}, bucket={s3BucketName}, keyPrefix={folderName}");
                var response = await s3Client.PutObjectAsync(putRequest);
                return Results.Ok($"Logs saved successfully to S3 at {folderName}/{fileName}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving logs to S3: {ex.Message}");
            }
        }// .SaveResourceToS3

    }// .S3FileService



}
