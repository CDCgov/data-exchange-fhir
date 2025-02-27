using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace fhir_facade_tests.ServicesTests
{
    public class S3FileServiceTests
    {
        // Constants for file paths, names, and expected content
        private const string S3StorageTestFolder = "s3storagetest";
        private const string S3FolderTest = "s3foldertest";
        private const string S3ResourceTest = "s3resourcetest";
        private const string S3FileName = "s3filename.json";

        public const string ExpectedFileContent = @"{
            ""name"": ""John Doe"",
            ""age"": 30,
            ""email"": ""john.doe@example.com"",
            ""isActive"": true,
            ""address"": {
                ""street"": ""123 Main St"",
                ""city"": ""Springfield"",
                ""state"": ""IL"",
                ""zipCode"": ""62701""
            },
            ""phoneNumbers"": [
                ""123-456-7890"",
                ""987-654-3210""
            ]
        }";

        [SetUp]
        public void Setup() { }

        [Test]
        public async Task TestS3FileService()
        {
            // Mock dependencies
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            var requestId = Guid.NewGuid().ToString();
            var mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);

            var mockS3Client = new Mock<AmazonS3Client>();

            // Setup the mock to capture the file content
            mockS3Client
                .Setup(client => client.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .Callback<PutObjectRequest, System.Threading.CancellationToken>((putRequest, cancellationToken) =>
                {
                    Console.WriteLine($"Captured file content: {putRequest.ContentBody}");
                    // Assert the file content is as expected
                    Assert.That(putRequest.ContentBody, Is.EqualTo(ExpectedFileContent));
                })
                .ReturnsAsync(new PutObjectResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                });

            // Set the configuration for the S3 bucket
            AwsConfig.BucketName = "BUCKET";
            AwsConfig.S3Client = mockS3Client.Object;

            // Create the S3FileService and call the SaveResource method
            var s3FileService = new S3FileService(mockLoggingUtility.Object);
            await s3FileService.SaveResource( S3ResourceTest, S3FileName, ExpectedFileContent);
        }
    }
}
