using Hl7.Fhir.Model.CdsHooks;
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
using System.Net;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.ControllerTests
{
    [TestFixture]
    public class BundleControllerTests
    {
        //  private Mock<BundleController> _mockBundeService;
        private BundleController _controller;

        [SetUp]
        public void SetUp()
        {

              var secretKey = "your-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-key"; // Secret key for signing the token
              var issuer = "your-issuer"; // The issuer (usually your API or service name)
              var audience = "your-audience"; // The audience (target client or system)
              var expirationDate = DateTime.UtcNow.AddHours(1); // Set expiration time for the token



             string token = GenerateJwtToken(secretKey, issuer, audience, expirationDate);


            //  // Create the mock HttpContext
            var mockHttpContext = new Mock<HttpContext>();


              var mockHeaders = new Mock<IHeaderDictionary>();

            //  // Add the Authorization header to the mock headers
              mockHeaders.Setup(headers => headers.ContainsKey("Authorization")).Returns(true);
              mockHeaders.Setup(headers => headers["Authorization"]).Returns(new StringValues(token));

            //  mockHeaders
            //.Setup(h => h.TryGetValue("Authorization", out It.Ref<StringValues>.IsAny))
            //.Returns((string key, out StringValues value) =>
            //{
            //    if (key == "Authorization")
            //    {
            //        value = new StringValues(token);
            //        return true;
            //    }
            //    value = StringValues.Empty;
            //    return false;
            //});


            //  //   mockHeaders.Setup(headers => headers.TryGetValue("Authorization").Returns(new StringValues("Bearer some-jwt-token"));



            //  // Setup the Request's Headers property



            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);

            var bodyContent = getJsonBundle();

            //  // Set the Request.Body to a memory stream with the JSON content
            mockRequest.Setup(req => req.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(bodyContent)));

            //  // Set up the mock HttpContext.Request to return the mock request
            mockHttpContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            var loggerService = new Mock<LoggerService>();

            var logToS3BucketService = new Mock<ILogToS3BucketService>();

            string requestId = Guid.NewGuid().ToString(); 
           var mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object,requestId);


            LocalFileStorageConfig.UseLocalDevFolder = true;

            LocalFileStorageConfig.LocalDevFolder = ".";

            mockLoggingUtility.Setup(utility => utility.Logging("MESSAGE"));

            //  // Create the controller
            _controller = new BundleController(mockLoggingUtility.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };



          //  mockRequest.Setup(req => req.Headers.Authorization).Returns(token);

        }

        [Test]
        public async Task testSuccesfulPost() 
        {
            Console.WriteLine("POST");
            var result =  await _controller.Post();
         

            Console.WriteLine("POST" + result.GetType().FullName);
            Console.WriteLine("POST" + result.GetType().Name);
            Console.WriteLine("POST" + result.GetType().Name);


        }


        private string GenerateJwtToken(string secretKey, string issuer, string audience, DateTime expirationDate)
        {
            // Define the key used for signing
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create the signing credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the claims (you can add more claims if needed)
            var claims = new[]
            {
            new System.Security.Claims.Claim(ClaimTypes.Name, "userName"), // Add more claims as needed
            new System.Security.Claims.Claim(ClaimTypes.Role, "admin"),
            new System.Security.Claims.Claim("client_id", "7bojglo66k83fe8gugojugitao")

        };

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationDate,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            // Create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the token as a string
            return tokenHandler.WriteToken(token);
        }

        private string getJsonBundle()
        {
         
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\sample.json");

            try
            {
                // Read all text from the file
                string fileContent = File.ReadAllText(filePath);
                return fileContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return "";
        }
    }


  

}

