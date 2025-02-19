using Hl7.Fhir.Model.CdsHooks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.ControllerTests
{
    [TestFixture]
    public class BundleControllerTests
    {
        private BundleController _controller;

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
            var mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);

            LocalFileStorageConfig.UseLocalDevFolder = true;
            LocalFileStorageConfig.LocalDevFolder = ".";

            mockLoggingUtility.Setup(utility => utility.Logging("MESSAGE"));

            _controller = new BundleController(mockLoggingUtility.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Test]
        public async Task TestSuccessfulPost()
        {
            var result = await _controller.Post();
            Assert.That(result, Is.Not.Null);
        }

        private string GenerateJwtToken(string secretKey, string issuer, string audience, DateTime expirationDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "userName"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim("client_id", "7bojglo66k83fe8gugojugitao")
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
