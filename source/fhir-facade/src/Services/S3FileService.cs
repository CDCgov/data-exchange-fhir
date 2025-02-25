using Amazon.S3.Model;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
using System;
using System.Threading.Tasks;

namespace OneCDPFHIRFacade.Services
{
    public class S3FileService : IFileService
    {
        private readonly LoggingUtility _loggingUtility;

        public S3FileService(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility ?? throw new ArgumentNullException(nameof(loggingUtility));
        }

        public async Task<IResult> SaveResource( string resourceType, string fileName, string content)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = AwsConfig.BucketName,
                Key = $"{resourceType}/{fileName}",
                ContentBody = content
            };

            try
            {
                // Log the start of the save process
                var logMessage = $"Start writing resource to S3: fileName={fileName}, bucket={AwsConfig.BucketName}/{resourceType}";
                await _loggingUtility.Logging(logMessage);
                Console.WriteLine(logMessage);

                // Perform the S3 upload
                var response = await AwsConfig.S3Client!.PutObjectAsync(putRequest);

                // Log the successful upload
                var logString = $"Resource saved to S3: fileName={fileName}, bucket={AwsConfig.BucketName}/{resourceType}, response={response.HttpStatusCode}";
                await _loggingUtility.Logging(logString);
                Console.WriteLine(logString);

                // Save log to S3 and return success result
                await _loggingUtility.SaveLogS3(fileName);
                return Results.Ok($"Resource saved successfully to S3 at {resourceType}/{fileName}");
            }
            catch (Exception ex)
            {
                // Log the error and return failure result
                var errorMessage = $"Error saving resource to S3: {ex.Message}";
                await _loggingUtility.Logging(errorMessage);
                await _loggingUtility.SaveLogS3(fileName);
                return Results.Problem(errorMessage);
            }
        }
    }
}
