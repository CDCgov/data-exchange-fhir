// S3FileService.cs
// #####################################################
// SaveResourceToS3
// #####################################################

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace OneCDPFHIRFacade.Services
{
        public interface IS3FileService
    {
        Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string bucketName, string folderName, string fileName, string content);
         // TODO: should this be IActionResult? vs. IResult 
    }
    public class S3FileService  : IS3FileService
    {
        public async Task<IResult> SaveResourceToS3(IAmazonS3 s3Client, string s3BucketName, string keyPrefix, string fileName, string resourceJson)
        {

            // Define the S3 put request
            var putRequest = new PutObjectRequest
            {
                BucketName = s3BucketName,
                Key = $"{keyPrefix}/{fileName}",
                ContentBody = resourceJson
            };

            // Attempt to save the resource to S3
            try
            {
                Console.WriteLine($"Start write to S3: fileName={fileName}, bucket={s3BucketName}, keyPrefix={keyPrefix}");

                var response = await s3Client.PutObjectAsync(putRequest);
                
                Console.WriteLine($"End write to S3: fileName={fileName}, response={response.HttpStatusCode}");

                return Results.Ok($"Resource saved successfully to S3 at {keyPrefix}/{fileName}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving resource to S3: {ex.Message}");
            }
        }// .SaveResourceToS3

    }// .S3FileService

} // .namespace