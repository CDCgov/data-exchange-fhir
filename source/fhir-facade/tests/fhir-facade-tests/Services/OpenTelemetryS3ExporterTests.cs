using Microsoft.AspNetCore.Http;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;
using OpenTelemetry;
using System.Diagnostics;

namespace fhir_facade_tests.ServicesTests
{
    public class OpenTelemetryS3ExporterTests
    {
        private Mock<LoggingUtility> _mockLoggingUtility;
        private Mock<IFileService> _mockFileService;
        private Mock<FileServiceFactory> _mockFileServiceFactory;
        [SetUp]
        public void Initialize()
        {
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            _mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, "123");
            _mockFileService = new Mock<IFileService>();
            _mockFileServiceFactory = new Mock<FileServiceFactory>(_mockLoggingUtility.Object);
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenLoggingUtilityIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OpenTelemetryS3Exporter(null!, _mockFileServiceFactory.Object));
        }

        [Test]
        public void Export_ShouldNotThrowException_WhenBatchIsEmpty()
        {
            // Arrange
            var batch = new Batch<Activity>(Array.Empty<Activity>(), 0);
            var _exporter = new OpenTelemetryS3Exporter(_mockLoggingUtility.Object, _mockFileServiceFactory.Object);


            // Act
            var result = _exporter.Export(batch);

            // Assert
            Assert.That(result, Is.EqualTo(ExportResult.Success));
            _mockFileService.Verify(f => f.OpenTelemetrySaveResource(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Export_ShouldSaveResource_WhenS3ClientIsSet()
        {
            // Arrange
            AwsConfig.S3Client = new Amazon.S3.AmazonS3Client(); // Simulating an S3 client being set
            var _exporter = new OpenTelemetryS3Exporter(_mockLoggingUtility.Object, _mockFileServiceFactory.Object);
            var activity = new Activity("TestActivity");
            activity.Start(); // Generates an ID automatically

            var batch = new Batch<Activity>(new[] { activity }, 1);

            _mockFileServiceFactory
                .Setup(f => f.CreateFileService(false)) // When S3Client is set, it should create an S3 file service
                .Returns(_mockFileService.Object);

            // Act
            var result = _exporter.Export(batch);

            // Assert
            Assert.That(result, Is.EqualTo(ExportResult.Success));
            Assert.That(AwsConfig.S3Client, Is.Not.Null);

            _mockFileService.Verify(f => f.OpenTelemetrySaveResource("Activity", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Export_ShouldSaveResourceLocally_WhenS3ClientIsNotSet()
        {
            // Arrange
            AwsConfig.S3Client = null; // Simulating S3Client being null
            Console.WriteLine($"S3Client is null: {AwsConfig.S3Client == null}");
            _mockFileServiceFactory
                   .Setup(f => f.CreateFileService(true)) // Ensures that a local file service is created
                   .Returns(_mockFileService.Object)
                   .Verifiable();

            var _exporter = new OpenTelemetryS3Exporter(_mockLoggingUtility.Object, _mockFileServiceFactory.Object);

            var activity = new Activity("TestActivity");
            activity.Start();
            var batch = new Batch<Activity>(new[] { activity }, 1);

            _mockFileService
                .Setup(f => f.OpenTelemetrySaveResource(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string category, string fileName, string content) =>
                {
                    Console.WriteLine($"SaveResource called with: {category}, {fileName}");
                })
                .Returns(Task.FromResult<IResult>(Results.Ok("")));

            // Act
            var result = _exporter.Export(batch);

            // Assert
            Assert.That(result, Is.EqualTo(ExportResult.Success));

            // Verify that CreateFileService(true) was called once
            _mockFileServiceFactory.Verify(f => f.CreateFileService(true), Times.Once);

            // Verify that SaveResource was called
            _mockFileService.Verify(
                f => f.OpenTelemetrySaveResource("Activity", It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );
        }
    }
}