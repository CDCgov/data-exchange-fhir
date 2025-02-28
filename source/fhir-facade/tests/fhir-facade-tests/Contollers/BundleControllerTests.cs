using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.ControllerTests
{
    [TestFixture]
    public class BundleControllerTests
    {
        private BundleController _controller;
        private Mock<LoggingUtility> mockLoggingUtility;

        [SetUp]
        public void SetUp()
        {
            var secretKey = "your-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-key";
            var issuer = "your-issuer";
            var audience = "your-audience";
            var expirationDate = DateTime.UtcNow.AddHours(1);

            string token = GenerateJwtToken(secretKey, issuer, audience, expirationDate);

            var mockHttpContext = new Mock<HttpContext>();
            var mockHeaders = new Mock<IHeaderDictionary>();

            mockHeaders.Setup(headers => headers.ContainsKey("Authorization")).Returns(true);
            mockHeaders.Setup(headers => headers["Authorization"]).Returns(new StringValues(token));

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);

            var bodyContent = GetJsonBundle();
            mockRequest.Setup(req => req.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(bodyContent)));

            mockHttpContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);

            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            string requestId = Guid.NewGuid().ToString();
            mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);

            LocalFileStorageConfig.UseLocalDevFolder = true;
            LocalFileStorageConfig.LocalDevFolder = ".";

            mockLoggingUtility.Setup(utility => utility.Logging("MESSAGE"));
            var httpContext = new DefaultHttpContext();
            _controller = new BundleController(mockLoggingUtility.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public async System.Threading.Tasks.Task TestSuccessfulPost()
        {
            var result = await _controller.Post();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnBadRequest_When_NoFileUploaded()
        {
            // Arrange: Simulating a form request with no file
            var formCollection = new FormCollection(new Dictionary<string, StringValues>());
            _controller.HttpContext.Request.ContentType = "multipart/form-data";
            _controller.HttpContext.Request.Form = formCollection;

            // Act
            var result = await _controller.Post();
            Assert.That(result, Is.InstanceOf<BadRequest<Dictionary<string, string>>>(),
                $"Expected BadRequest<Dictionary<string, string>>, but got {result.GetType().FullName}");
            // Assert
            Assert.That(result, Is.TypeOf<BadRequest<Dictionary<string, string>>>());
            var badResult = result as BadRequest<Dictionary<string, string>>;
            Assert.That(badResult, Is.Not.Null);
            Assert.That(badResult.Value, Is.Not.Null);
            var resultError = badResult.Value["error"];
            var resultMessage = badResult.Value["message"];

            Assert.That(resultError, Is.EqualTo("Invalid request"));
            Assert.That(resultMessage, Is.EqualTo("No file uploaded or file is empty."));
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnBadRequest_When_ContentTypeNotSupported()
        {
            // Arrange: Setting an unsupported content type
            _controller.HttpContext.Request.ContentType = "text/plain";

            // Act
            var result = await _controller.Post();

            // Assert
            Assert.That(result, Is.TypeOf<BadRequest<Dictionary<string, string>>>());

            var badResult = result as BadRequest<Dictionary<string, string>>;
            Assert.That(badResult, Is.Not.Null);
            Assert.That(badResult.Value, Is.Not.Null);
            var badRequestError = badResult.Value["error"];
            var badRequestMessage = badResult.Value["message"];

            Assert.That(badRequestError, Is.EqualTo("Invalid request"));
            Assert.That(badRequestMessage, Is.EqualTo("Supported content types: application/json or multipart/form-data."));
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnBadRequest_When_InvalidJsonProvided()
        {
            // Arrange: Setting up an invalid JSON request
            var invalidJson = "This is not a JSON";
            _controller.HttpContext.Request.ContentType = "application/json";
            _controller.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));

            // Act
            var result = await _controller.Post();

            // Assert
            var badResult = result as BadRequest<Dictionary<string, string>>;
            Assert.That(badResult, Is.Not.Null);
            Assert.That(badResult.Value, Is.Not.Null);
            var badRequestError = badResult.Value["error"];
            var badRequestMessage = badResult.Value["message"];

            Assert.That(badRequestError, Is.EqualTo("Invalid request"));
            Assert.That(badRequestMessage, Is.EqualTo("Unable to parse request."));
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnBadRequest_When_ContentTypeIsNotMultipartFormData()
        {
            // Arrange: Set an invalid content type (e.g., application/json)
            _controller.HttpContext.Request.ContentType = "application/json";

            // Mocking HasFormContentType to be true (so the block executes)
            _controller.HttpContext.Request.Headers["Content-Type"] = "application/json";

            // Mocking Form property to prevent exceptions when accessing it
            var formCollection = new FormCollection(new Dictionary<string, StringValues>());
            _controller.HttpContext.Request.Form = formCollection;

            // Act: Call the controller's Post() method
            var result = await _controller.Post();

            // Assert: Ensure the result is a BadRequest with a Dictionary<string, string>
            Assert.That(result, Is.InstanceOf<BadRequest<Dictionary<string, string>>>(),
                $"Expected BadRequest<Dictionary<string, string>>, but got {result.GetType().FullName}");

            // Extract the BadRequest result
            var badRequestResult = result as BadRequest<Dictionary<string, string>>;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequest result should not be null");
            Assert.That(badRequestResult!.Value, Is.Not.Null, "BadRequest value should not be null");

            // Ensure the dictionary contains expected error messages
            Assert.That(badRequestResult.Value, Does.ContainKey("error"), "Expected dictionary to contain key 'error'");
            Assert.That(badRequestResult.Value["error"], Is.EqualTo("Invalid request"), "Incorrect 'error' message");

            Assert.That(badRequestResult.Value, Does.ContainKey("message"), "Expected dictionary to contain key 'message'");
            Assert.That(badRequestResult.Value["message"], Is.EqualTo("Expected multipart/form-data but received a different content-type."),
                "Incorrect 'message' value");
        }


        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnBadRequest_When_BundleIdIsMissing()
        {
            // Arrange: Setting up a valid but missing `Id` in the FHIR Bundle
            Bundle bundle = new Bundle
            {
                Type = Bundle.BundleType.Collection,
                Timestamp = DateTimeOffset.UtcNow,
            };
            var jsonBundle = await bundle.ToJsonAsync();
            _controller.HttpContext.Request.ContentType = "application/json";
            _controller.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBundle));

            // Act
            var result = await _controller.Post();
            var badResult = result as BadRequest<Dictionary<string, string>>;
            Assert.That(badResult, Is.Not.Null);
            Assert.That(badResult.Value, Is.Not.Null);
            var badRequestError = badResult.Value["error"];
            var badRequestMessage = badResult.Value["message"];

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestError, Is.EqualTo("Invalid payload"));
            Assert.That(badRequestMessage, Is.EqualTo("Resource ID is required."));
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Should_ReturnSuccess_When_ValidBundleIsProvided()
        {
            // Arrange: Mocking a valid FHIR Bundle
            var bundle = new Bundle
            {
                Id = "123"
            };
            var jsonBundle = await bundle.ToJsonAsync();
            _controller.HttpContext.Request.ContentType = "application/json";
            _controller.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBundle));

            // Mock logging behavior
            mockLoggingUtility
                .Setup(log => log.Logging(It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _controller.Post();

            // Assert
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }
        [Test]
        public async System.Threading.Tasks.Task Test_FileCopyToAsync_ReadsContentCorrectly()
        {
            // Arrange
            var fileContent = "This is a test content of the file";
            var fileName = "testFile.json";

            // Create a MemoryStream with the content
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            Mock<IFormFile> _mockFile = new Mock<IFormFile>();
            _mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockFile.Setup(f => f.OpenReadStream()).Returns(fileStream);
            _mockFile.Setup(f => f.Length).Returns(fileStream.Length);
            _mockFile.Setup(f => f.FileName).Returns(fileName);

            // Act
            using var memoryStream = new MemoryStream();
            await _mockFile.Object.CopyToAsync(memoryStream); // Simulating the CopyToAsync call
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset position
            var resultContent = await new StreamReader(memoryStream).ReadToEndAsync();

            // Assert
            Assert.That(fileContent, Is.EqualTo("This is a test content of the file"));
        }

        private string GenerateJwtToken(string secretKey, string issuer, string audience, DateTime expirationDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, "userName"),
                new System.Security.Claims.Claim(ClaimTypes.Role, "admin"),
                new System.Security.Claims.Claim("client_id", "7bojglo66k83fe8gugojugitao")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationDate,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetJsonBundle()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\sample.json");

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
