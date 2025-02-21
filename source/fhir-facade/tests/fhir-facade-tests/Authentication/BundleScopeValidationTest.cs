using Amazon.CloudWatchLogs;
using Hl7.Fhir.Model;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.Authentication
{
    [TestFixture]
    public class BundleScopeValidationTest
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
            _loggerService = new LoggerService(_mockCloudWatchLogsClient.Object, "TestUserScope");
            _logToS3BucketService = new LogToS3BucketService();

        }

        [Test]
        public async System.Threading.Tasks.Task BundleScopeValidator_Scope_Match_BundleProfile()
        {
            // Arrange

            var loggingUtility = new LoggingUtility(_loggerService, _logToS3BucketService, "123");

            Bundle bundle = new Bundle
            {
                Type = Bundle.BundleType.Collection,
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                Meta = new Meta
                {
                    Profile = ["http://hl7.org/fhir/us/ecr/StructureDefinition/eicr-composition"]
                }
            };

            string[] scopeClaim = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            // Create an instance of BundleScopeValidation with mock dependencies
            BundleScopeValidation bundleScopeValidation = new BundleScopeValidation(bundle, loggingUtility);

            // Act
            bool isValid = await bundleScopeValidation.IsBundleProfileMatchScope(scopeClaim);

            // Assert - Request IDs should be unique for each log entry
            Assert.That(isValid);
        }

        [Test]
        public async System.Threading.Tasks.Task BundleScopeValidator_Scope_DoesNotMatch_BundleProfile()
        {
            // Arrange
            var loggingUtility = new LoggingUtility(_loggerService, _logToS3BucketService, "123");

            Bundle bundle2 = new Bundle
            {
                Type = Bundle.BundleType.Collection,
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                Meta = new Meta
                {
                    Profile = ["http://hl7.org/fhir/us/medmorph/StructureDefinition/us-ph-content-bundle"]
                }
            };
            string[] scopeClaim = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };

            // Create an instance of BundleScopeValidation with mock dependencies
            BundleScopeValidation bundleScopeValidation = new BundleScopeValidation(bundle2, loggingUtility);

            // Act
            bool isNotValid = await bundleScopeValidation.IsBundleProfileMatchScope(scopeClaim);

            // Act & Assert
            Assert.That(!isNotValid);
        }
    }

}

