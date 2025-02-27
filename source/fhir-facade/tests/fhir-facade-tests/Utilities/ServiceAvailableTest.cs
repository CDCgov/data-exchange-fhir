using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.Utilities
{
    [TestFixture]
    public class ServiceAvailableTest
    {

        private Mock<AmazonCloudWatchLogsClient> _mockCloudWatchLogsClient;
        private Mock<AmazonS3Client> _mockS3Client;
        private ServiceAvailabilityUtility _serviceAvailability;

        [SetUp]
        public void SetUp()
        {
            // Mock CloudWatch Logs client and mock S3 client to prevent actual AWS interactions
            _mockCloudWatchLogsClient = new Mock<AmazonCloudWatchLogsClient>();
            _mockS3Client = new Mock<AmazonS3Client>();

            // Assign the mocks to the AwsConfig (simulate dependency injection)
            AwsConfig.logsClient = _mockCloudWatchLogsClient.Object;
            AwsConfig.S3Client = _mockS3Client.Object;

            AwsConfig.LogGroupName = "test-log-group";
            AwsConfig.BucketName = "test-bucket";

            // Create instance of ServiceAvailabilityUtility
            _serviceAvailability = new ServiceAvailabilityUtility();
        }

        [Test]
        public async Task ServiceAvailable_Should_Return_LogServiceAvailable_When_LogClient_Is_NotNull()
        {
            // Arrange:
            _mockCloudWatchLogsClient
                .Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
                .ReturnsAsync(new DescribeLogStreamsResponse());

            // Act: Get request IDs
            var result = await _serviceAvailability.ServiceAvailable();


            // Assert: Ensure they are different
            Assert.That(result.Contains("Log Service is available and healthy."));
        }

        [Test]
        public async Task ServiceAvailable_Should_Return_LogServiceUnAvailable_When_LogClient_Is_Null()
        {
            // Arrange:
            AwsConfig.logsClient = null;

            // Act: Get request IDs
            var result = await _serviceAvailability.ServiceAvailable();


            // Assert: Ensure they are different
            Assert.That(result.Contains("Log Service is unavailable."));
        }

        [Test]
        public async Task ServiceAvailable_Should_Return_S3Available_When_BucketExists()
        {
            // Arrange:
            _mockS3Client
                .Setup(client => client.ListBucketsAsync(default))
                .ReturnsAsync(new ListBucketsResponse
                {
                    Buckets = new List<S3Bucket> { new S3Bucket { BucketName = "test-bucket" } }
                });

            // Act: Get request IDs
            var result = await _serviceAvailability.ServiceAvailable();

            // Assert: Ensure they are different
            Assert.That(result.Contains("S3 Bucket is available and healhy."));
        }
        [Test]
        public async Task ServiceAvailable_Should_Return_S3Unavailable_When_BucketMissing()
        {
            // Arrange
            _mockS3Client
                .Setup(client => client.ListBucketsAsync(default))
                .ReturnsAsync(new ListBucketsResponse
                {
                    Buckets = new List<S3Bucket>() // Empty bucket list
                });

            // Act: Get request IDs
            var result = await _serviceAvailability.ServiceAvailable();

            // Assert: Ensure they are different
            Assert.That(result.Contains("S3 Bucket is unavailable."));
        }

        [Test]
        public async Task ServiceAvailable_Should_Return_FailedMessage_On_Exception()
        {
            // Arrange
            _mockCloudWatchLogsClient
                .Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
                .ThrowsAsync(new System.Exception("AWS Service Error"));

            // Act
            var result = await _serviceAvailability.ServiceAvailable();

            // Assert
            Assert.That(result.Contains("Failed to get access to services."));
        }

    }
}

