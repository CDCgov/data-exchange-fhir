using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using OneCDPFHIRFacade.Services;

namespace fhir_facade_tests.Services
{
    public class LogToS3FileServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1Async()
        {
            var mockS3Client = new Mock<IAmazonS3>();

            // Set up the mock to return a successful response
            mockS3Client
                .Setup(client => client.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(new PutObjectResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                });

            var logToS3FileService = new LogToS3FileService();

            var response = await logToS3FileService.SaveResourceToS3(mockS3Client.Object,"bucket","filename","id");
           Console.WriteLine("response here" + response);

            Assert.Pass();
        }
    }
}