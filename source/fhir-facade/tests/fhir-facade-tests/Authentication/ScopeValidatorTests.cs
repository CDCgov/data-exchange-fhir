using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System;
using System.Threading.Tasks;

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
            string scopeClaim = "Banana";
            string[] requiredScopes = new string[] { "Apple", "Banana", "Cherry" };

            var result = await _controller.Validate(scopeClaim, requiredScopes);
            Assert.That(result);
        }

        [Test]
        public async Task TestDoesNotValidate()
        {
            string scopeClaim = "HAS";
            string[] requiredScopes = new string[] { "Apple", "Banana", "Cherry" };

            var result = await _controller.Validate(scopeClaim, requiredScopes);
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
