using Amazon.S3.Model;
using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Services
{
    public class LogToS3FileService
    {
        readonly List<string> resultList = new List<string>();
        public async Task<IResult> SaveResourceToS3(string result, string fileName)
        {
            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = $"{AwsConfig.BucketName}",
                Key = $"Logs/logs{fileName}.json",
                ContentBody = result
            };

            // Attempt to save the resource to S3
            try
            {
                await AwsConfig.S3Client!.PutObjectAsync(putRequest);
                Console.WriteLine(result);
                return Results.Ok($"Logs saved successfully to S3 Logs");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving logs to S3: {ex.Message}");
            }
        }// .SaveResourceToS3
    }
}