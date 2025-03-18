using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.ServicesTests
{
    public class FileServiceFactoryTests
    {
        private readonly Mock<LoggingUtility> _mockLoggingUtility;
        private readonly FileServiceFactory _fileServiceFactory;


        public FileServiceFactoryTests()
        {
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            var requestId = Guid.NewGuid().ToString();

            _mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);
            _fileServiceFactory = new FileServiceFactory(_mockLoggingUtility.Object);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggingUtilityIsNull()
        {
            Assert.That(() => new FileServiceFactory(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFileService_ReturnsLocalFileService_WhenRunLocallyIsTrue()
        {
            var fileService = _fileServiceFactory.CreateFileService(true);
            Assert.That(fileService, Is.InstanceOf<LocalFileService>());
        }

        [Test]
        public void CreateFileService_ReturnsS3FileService_WhenRunLocallyIsFalse()
        {
            var fileService = _fileServiceFactory.CreateFileService(false);
            Assert.That(fileService, Is.InstanceOf<S3FileService>());
        }

        [Test]
        public async Task SaveResource_ReturnsOkResult()
        {
            var result = await _fileServiceFactory.SaveResource("Patient", "test.json", "{}");
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }

        [Test]
        public async Task OpenTelemetrySaveResource_ReturnsOkResult()
        {
            var result = await _fileServiceFactory.OpenTelemetrySaveResource("Patient", "test.json", "{}");
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }
    }
}
