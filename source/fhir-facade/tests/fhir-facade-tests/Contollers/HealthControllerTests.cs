using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.ControllerTests
{
    [TestFixture]
    public class HealthControllerTests
    {

        private Mock<IServiceAvailabilityUtility> _mockServiceAvailabilityUtility;
        private HealthController _controller;
        [SetUp]
        public void SetUp()
        {
            // Mock the ServiceAvailabilityUtility
            _mockServiceAvailabilityUtility = new Mock<IServiceAvailabilityUtility>();
            _controller = new HealthController(_mockServiceAvailabilityUtility.Object);
        }

        [Test]
        public void TestGet()
        {
            var result = _controller.GetHealth;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetHealth_ShouldReturnHealthyStatus()
        {
            // Act
            var result = _controller.GetHealth();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<JsonHttpResult<Dictionary<string, string>>>());
            var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.Value, Is.Not.Null);
            var resultStatic = jsonResult.Value["status"];

            Assert.That(resultStatic, Is.EqualTo("Healthy"));
        }
        [Test]
        public async Task GetAwsServiceHealth_ShouldReturnServiceAvailable_WhenNoUnavailableServices()
        {

            // Arrange
            var mockServiceList = new List<string> { "Log Service is available and healhy.", "S3 Bucket is available and healhy." };
            _mockServiceAvailabilityUtility.Setup(s => s.ServiceAvailable()).ReturnsAsync(mockServiceList);

            // Act
            var result = await _controller.GetAwsServiceHealth();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Ok<Dictionary<string, string>>>());
            var okResult = result as Ok<Dictionary<string, string>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            var resultStatic = okResult.Value["status"];

            Assert.That(resultStatic, Is.EqualTo("Available"));
        }

        [Test]
        public async Task GetAwsServiceHealth_ShouldReturnServiceUnavailable_WhenUnavailableServices()
        {

            // Arrange
            var mockServiceList = new List<string> { "Log Service is unavailable.", "S3 Bucket is available and healthy." };
            _mockServiceAvailabilityUtility.Setup(s => s.ServiceAvailable()).ReturnsAsync(mockServiceList);

            // Act
            var result = await _controller.GetAwsServiceHealth();

            // Assert
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>()); // Correct type assertion

            var problemResult = result as ProblemHttpResult;
            Assert.That(problemResult, Is.Not.Null);
            Assert.That(problemResult.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));
            Assert.That(problemResult.ProblemDetails.Detail, Is.EqualTo("Log Service is unavailable. S3 Bucket is available and healthy."));
        }

        [TearDown]
        public void TearDown()
        {

        }

    }

}

