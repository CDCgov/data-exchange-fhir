// S3FileService.cs

using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class S3FileService
{

    // #####################################################
    // SaveResourceToS3
    // #####################################################
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
