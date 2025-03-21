using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;
using System.Net;

namespace OneCDPFHIRFacade.Tests.Services
{
    [TestFixture]
    public class S3FileServiceTests
    {
        private Mock<AmazonS3Client> _mockS3Client;
        private Mock<LoggingUtility> _mockLoggingUtility;
        private S3FileService _s3FileService;
        private const string BucketName = "test-bucket";
        private const string ResourceType = "Patient";
        private const string FileName = "test.json";
        private const string Content = "{}";

        [SetUp]
        public void SetUp()
        {
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            var requestId = Guid.NewGuid().ToString();

            _mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);
            _mockS3Client = new Mock<AmazonS3Client>();
            _s3FileService = new S3FileService(_mockLoggingUtility.Object);

            // Set up the AwsConfig with the mock S3 client and bucket name
            AwsConfig.S3Client = _mockS3Client.Object;
            AwsConfig.BucketName = BucketName;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggingUtilityIsNull()
        {
            Assert.That(() => new S3FileService(null!), Throws.ArgumentNullException);
        }

        [Test]
        public async Task SaveResource_ReturnsOkResult_WhenS3UploadSucceeds()
        {
            // Arrange
            var putResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
            _mockS3Client.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ReturnsAsync(putResponse);

            // Act
            var result = await _s3FileService.SaveResource(ResourceType, FileName, Content);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }

        [Test]
        public async Task SaveResource_ReturnsProblemResult_WhenS3UploadFails()
        {
            // Arrange
            var exceptionMessage = "S3 upload failed";
            _mockS3Client.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _s3FileService.SaveResource(ResourceType, FileName, Content);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
        }

        [Test]
        public async Task OpenTelemetrySaveResource_ReturnsOkResult_WhenS3UploadSucceeds()
        {
            // Arrange
            var putResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
            _mockS3Client.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ReturnsAsync(putResponse);

            // Act
            var result = await _s3FileService.OpenTelemetrySaveResource(ResourceType, FileName, Content);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }

        [Test]
        public async Task OpenTelemetrySaveResource_ReturnsProblemResult_WhenS3UploadFails()
        {
            // Arrange
            var exceptionMessage = "S3 upload failed";
            _mockS3Client.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _s3FileService.OpenTelemetrySaveResource(ResourceType, FileName, Content);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
        }
    }
}
