using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.Net;

namespace OneCDPFHIRFacade.Tests
{
    public class HealthControllerTests
    {
        [Test]
        public void GetHealth_ReturnsHealthyStatus()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.GetHealth();
            var okResult = result as Ok<Dictionary<string, string>>;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            var okRequestMessage = okResult.Value["status"];
            Assert.That(okRequestMessage, Is.EqualTo("Healthy"));

        }

        [Test]
        public async Task GetAwsServiceHealth_ReturnsAvailableStatus()
        {
            // Arrange
            var mockServiceUtility = new Mock<IServiceAvailabilityUtility>();
            mockServiceUtility.Setup(s => s.ServiceAvailable())
                .ReturnsAsync(new List<string> { "Service A Available", "Service B Available" });

            var controller = new TestableHealthController(mockServiceUtility.Object);

            // Act
            var result = await controller.GetAwsServiceHealth();
            var okResult = result as Ok<Dictionary<string, string>>;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);

            var okRequeststatus = okResult.Value["status"];
            var okRequestMessage = okResult.Value["description"];

            // Assert
            Assert.That(okRequestMessage, Is.EqualTo("Service A Available Service B Available"));
        }

        [Test]
        public async Task GetAwsServiceHealth_ReturnsServiceUnavailable()
        {
            // Arrange
            var mockServiceUtility = new Mock<IServiceAvailabilityUtility>();
            mockServiceUtility.Setup(s => s.ServiceAvailable())
                .ReturnsAsync(new List<string> { "Service A Unavailable", "Service B Available" });

            var controller = new HealthController();

            // Act
            var result = await controller.GetAwsServiceHealth();

            // Assert
            var problemResult = result as ProblemHttpResult;
            Assert.That(problemResult, Is.Not.Null);
            Assert.That(problemResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.ServiceUnavailable));

        }
    }
    public class TestableHealthController : HealthController
    {
        private readonly IServiceAvailabilityUtility _serviceAvailabilityUtility;

        public TestableHealthController(IServiceAvailabilityUtility serviceAvailabilityUtility)
        {
            _serviceAvailabilityUtility = serviceAvailabilityUtility;
        }

        public new virtual async Task<IResult> GetAwsServiceHealth()
        {
            List<string> serviceAvailable = await _serviceAvailabilityUtility.ServiceAvailable();
            string message = "";
            foreach (string item in serviceAvailable)
            {
                if (message.Length > 0)
                    message += " ";
                message += item;
            }
            if (!serviceAvailable.Any(s => s.Contains("unavailable")))
            {
                return Results.Ok(new Dictionary<string, string>
                {
                    {"status", "Availible" },
                    {"timestamp", DateTime.UtcNow.ToString("")},
                    {"description", message }
                });
            }
            else
            {
                return TypedResults.Problem(message, statusCode: (int)HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
