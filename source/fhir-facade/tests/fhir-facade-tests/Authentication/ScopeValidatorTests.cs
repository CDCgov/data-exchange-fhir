using Microsoft.Extensions.DependencyInjection;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.AuthenticationTests
{
    [TestFixture]
    public class ScopeValidatorTests
    {
        private ScopeValidator _controller;

        [SetUp]
        public void SetUp()
        {
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            string requestId = Guid.NewGuid().ToString();

            var mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);

            // Set up dependency injection for the controller
            var services = new ServiceCollection();
            services.AddSingleton(mockLoggingUtility.Object); // Register the LoggingUtility service

            var serviceProvider = services.BuildServiceProvider();
            _controller = new ScopeValidator(serviceProvider);
        }

        [Test]
        public async Task TestValidates()
        {
            string scopeClaimString = "system/bundle.c";
            AwsConfig.ScopeClaim = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            AwsConfig.ClientScope = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            var result = await _controller.Validate(scopeClaimString);
            Assert.That(result);
        }

        [Test]
        public async Task TestDoesNotValidate()
        {
            string scopeClaimString = "HAS";
            AwsConfig.ScopeClaim = new string[] { "system/bundle.rs", "org/org-name1", "stream/eicr-composition" };
            AwsConfig.ClientScope = new string[] { "system/bundle.c", "org/org-name1", "stream/eicr-composition" };
            var result = await _controller.Validate(scopeClaimString);
            Assert.That(!result);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean-up resources if needed
            // _controller?.Dispose();
        }
    }
}
