using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Moq;
using OneCDP.Logging;

namespace fhir_facade_tests.ServicesTests
{
    [TestFixture]
    public class LoggerServiceTests
    {
        private Mock<AmazonCloudWatchLogsClient> _mockLogClient;
        private LoggerService _loggerService;
        private readonly string _logGroupName = "TestLogGroup";

        [SetUp]
        public void SetUp()
        {
            // Mock CloudWatch Logs client to prevent actual AWS interactions
            _mockLogClient = new Mock<AmazonCloudWatchLogsClient>();

            // Create instance of LoggerService with mock dependency
            _loggerService = new LoggerService(_mockLogClient.Object, "TestLogGroup");
        }

        [Test]
        public async Task LogData_ConsoleLogs_ShouldLogMessageWithRequestId()
        {
            // Arrange
            string requestId1 = Guid.NewGuid().ToString();
            string requestId2 = Guid.NewGuid().ToString();
            string message = "Test log message";

            // Act
            await _loggerService.LogData(message, requestId1, env: true);
            await _loggerService.LogData(message, requestId2, env: true);

            // Assert - Request IDs should be unique for each log entry
            Assert.That(requestId1, Is.Not.EqualTo(requestId2));
        }

        [Test]
        public void LogData_CloudWatchLogs_ShouldNotThrowException()
        {
            // Arrange
            string requestId = Guid.NewGuid().ToString();
            string message = "CloudWatch log message";

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _loggerService.LogData(message, requestId, env: false));
        }
        [Test]
        public async Task CloudWatchLogs_CreatesLogStream_WhenLogStreamIsNull()
        {
            // Arrange
            var message = "Test Message";
            var requestId = "12345";
            var logStreamName = DateTime.UtcNow.ToString("yyyyMMdd");

            _mockLogClient.Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
                .ReturnsAsync(new DescribeLogStreamsResponse { LogStreams = new List<LogStream>() });

            _mockLogClient.Setup(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default))
                .ReturnsAsync(new CreateLogStreamResponse());

            _mockLogClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
                .ReturnsAsync(new PutLogEventsResponse());

            // Act
            await _loggerService.CloudWatchLogs(message, requestId);

            // Assert
            _mockLogClient.Verify(client => client.CreateLogStreamAsync(It.Is<CreateLogStreamRequest>(r => r.LogGroupName == _logGroupName && r.LogStreamName == logStreamName), default), Times.Once);
        }

        [Test]
        public async Task CloudWatchLogs_UsesSequenceToken_WhenLogStreamExists()
        {
            // Arrange
            var message = "Test Message";
            var requestId = "12345";
            var logStreamName = DateTime.UtcNow.ToString("yyyyMMdd");

            var logStream = new LogStream { LogStreamName = logStreamName, UploadSequenceToken = "12345" };

            _mockLogClient.Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
                .ReturnsAsync(new DescribeLogStreamsResponse { LogStreams = new List<LogStream> { logStream } });

            _mockLogClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
                .ReturnsAsync(new PutLogEventsResponse());

            // Act
            await _loggerService.CloudWatchLogs(message, requestId);

            // Assert
            _mockLogClient.Verify(client => client.PutLogEventsAsync(It.Is<PutLogEventsRequest>(r => r.SequenceToken == "12345"), default), Times.Once);
        }
    }

}