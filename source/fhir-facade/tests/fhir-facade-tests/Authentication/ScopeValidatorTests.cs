using Amazon.CloudWatchLogs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
using Task = System.Threading.Tasks.Task;

namespace fhir_facade_tests.AuthenticationTests
{
    [TestFixture]
    public class ScopeValidatorTests
    {
        private Mock<LoggingUtility> _mockLoggingUtility;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceScopeFactory> _mockScopeFactory;
        private Mock<ILogToS3BucketService> _mocklogToS3BucketService;
        private Mock<AmazonCloudWatchLogsClient> _mockCloudWatchLogsClient;
        private LoggerService _loggerService;


        [SetUp]
        public void SetUp()
        {
            _mockCloudWatchLogsClient = new Mock<AmazonCloudWatchLogsClient>();
            _loggerService = new LoggerService(_mockCloudWatchLogsClient.Object, "test");
            _mocklogToS3BucketService = new Mock<ILogToS3BucketService>();
            _mockLoggingUtility = new Mock<LoggingUtility>(_loggerService, _mocklogToS3BucketService.Object, "125");
            _mockScope = new Mock<IServiceScope>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);
            _mockScopeFactory.Setup(f => f.CreateScope()).Returns(_mockScope.Object);
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(_mockScopeFactory.Object);
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(LoggingUtility))).Returns(_mockLoggingUtility.Object);

        }

        [Test]
        public async Task TestValidates()
        {
            //Arrange
            string scopeClaimString = "system/bundle.c";
            AwsConfig.ScopeClaim = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            AwsConfig.ClientScope = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            var validate = new ScopeValidator(_mockServiceProvider.Object);

            //Act
            var result = await validate.Validate(scopeClaimString);

            Assert.That(result);
        }

        [Test]
        public async Task TestDoesNotValidate()
        {
            //Arrange
            string scopeClaimString = "HAS";
            AwsConfig.ScopeClaim = new string[] { "system/bundle.rs", "org/org-name1", "stream/eicr-composition" };
            AwsConfig.ClientScope = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            var validate = new ScopeValidator(_mockServiceProvider.Object);

            //Act
            var result = await validate.Validate(scopeClaimString);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Validate_ReturnsFalse_WhenScopeClaimIsNullOrEmpty()
        {
            // Arrange
            AwsConfig.ScopeClaim = null;
            var validator = new ScopeValidator(_mockServiceProvider.Object);

            // Act
            var result = await validator.Validate("test-scope");

            // Assert
            Assert.That(result, Is.False);

        }
        [Test]
        public async Task Validate_ReturnsFalse_WhenScopeClaimNotContainSystem()
        {
            //Arrange
            string scopeClaimString = "TEST";
            AwsConfig.ScopeClaim = new string[] { "hello/bundle.rs", "org/org-name1", "stream/eicr-composition" };
            AwsConfig.ClientScope = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            var validate = new ScopeValidator(_mockServiceProvider.Object);

            //Act
            var result = await validate.Validate(scopeClaimString);

            //Assert
            Assert.That(result, Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean-up resources if needed
            // _controller?.Dispose();
        }
    }
}
