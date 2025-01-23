// S3FileService.cs
// #####################################################
// SaveResourceToS3
// #####################################################

using Amazon.S3;
using Amazon.S3.Model;
using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Services
{
    public interface IS3FileService
    {
        Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string folderName, string fileName, string content, LoggerService logEntry, string requestId);
        // TODO: should this be IActionResult? vs. IResult 
    }
    public class S3FileService : IS3FileService
    {
        public async Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string s3BucketName, string keyPrefix, string fileName, string resourceJson, LoggerService logEntry, string requestId)
        {

            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = s3BucketName,
                Key = $"{keyPrefix}/{fileName}",
                ContentBody = resourceJson
            };
            LogToS3FileService logToS3FileService = new LogToS3FileService();

            // Attempt to save the resource to S3
            try
            {
                await logEntry.CloudWatchLogs($"Start write to S3: fileName={fileName}, " +
                    $"bucket={s3BucketName}, keyPrefix={keyPrefix}", requestId);
                logToS3FileService.JsonResult($"End writing to S3: fileName={fileName}, bucket={AwsConfig.BucketName}", requestId);

                Console.WriteLine($"Start write to S3: fileName={fileName}, bucket={s3BucketName}, keyPrefix={keyPrefix}");

                var response = await s3Client.PutObjectAsync(putRequest);

                await logEntry.CloudWatchLogs($"End write to S3: fileName={fileName}, " +
                    $"response={response.HttpStatusCode}", requestId);
                logToS3FileService.JsonResult($"End write to S3: fileName={fileName}, response={response.HttpStatusCode}", requestId);

                Console.WriteLine($"End write to S3: fileName={fileName}, response={response.HttpStatusCode}");

                await logToS3FileService.SaveResourceToS3(AwsConfig.S3Client!, AwsConfig.BucketName!, fileName, requestId);
                return Results.Ok($"Resource saved successfully to S3 at {keyPrefix}/{fileName}");
            }
            catch (Exception ex)
            {
                await logEntry.CloudWatchLogs($"Error saving resource to S3: {ex.Message}", requestId);
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }// .SaveResourceToS3

    }// .S3FileService

} // .namespace
