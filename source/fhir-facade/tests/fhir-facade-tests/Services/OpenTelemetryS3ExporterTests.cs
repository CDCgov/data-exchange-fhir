using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using OneCDP.Logging;

namespace fhir_facade_tests.ServicesTests
{
    public class OpenTelemetryS3Exporter
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

            var logToS3FileService = new LogToS3BucketService();

            var response = await logToS3FileService.SaveResourceToS3(mockS3Client.Object, "bucket", "filename");
            Console.WriteLine("response here" + response);

            Assert.Pass();
        }
    }
}