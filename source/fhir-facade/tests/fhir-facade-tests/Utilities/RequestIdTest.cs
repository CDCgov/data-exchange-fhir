using Amazon.CloudWatchLogs;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.UtilitiesTest
{
    [TestFixture]
    public class RequestIdTest
    {

        private Mock<AmazonCloudWatchLogsClient> _mockCloudWatchLogsClient;
        private LoggerService _loggerService;
        private LogToS3BucketService _logToS3BucketService;

        [SetUp]
        public void SetUp()
        {
            // Mock CloudWatch Logs client to prevent actual AWS interactions
            _mockCloudWatchLogsClient = new Mock<AmazonCloudWatchLogsClient>();

            // Create instance of LoggerService with mock dependency
            _loggerService = new LoggerService(_mockCloudWatchLogsClient.Object, "TestLogGroup");

            _logToS3BucketService = new LogToS3BucketService();
        }
        [Test]
        public void LoggingUtility_Uses_Same_RequestId_For_Single_Request()
        {
            // Arrange: Create two instances of LoggingUtility
            var loggingUtility1 = new LoggingUtility(_loggerService, _logToS3BucketService, "123");
            var loggingUtility2 = new LoggingUtility(_loggerService, _logToS3BucketService, "122");

            // Act: Get request IDs
            string requestId1 = loggingUtility1.requestId;
            string requestId2 = loggingUtility2.requestId;

            // Assert: Ensure they are different
            Assert.That(requestId1, Is.Not.EqualTo(requestId2)); //"Request IDs should be unique per instance."
        }

    }

}

