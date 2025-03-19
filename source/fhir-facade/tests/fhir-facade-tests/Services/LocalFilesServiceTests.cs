using Moq;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.ServicesTests
{
    public class LocalFilesServiceTests
    {
        // Constants for file paths, names, and expected content
        private const string LocalStorageTestFolder = "localstoragetest";
        private const string LocalFolderTest = "localfoldertest";
        private const string LocalResourceTest = "localresourcetest";
        private const string LocalFileName = "localfilename.json";
        public const string ExpectedFileContent = @"{
            ""name"": ""John Doe"",
            ""age"": 30,
            ""email"": ""john.doe@example.com"",
            ""isActive"": true,
            ""address"": {
                ""street"": ""123 Main St"",
                ""city"": ""Springfield"",
                ""state"": ""IL"",
                ""zipCode"": ""62701""
            },
            ""phoneNumbers"": [
                ""123-456-7890"",
                ""987-654-3210""
            ]
        }";

        [SetUp]
        public void Setup()
        {
            // Setup code (currently not needed)
        }

        [Test]
        public async Task TestLocalFileService()
        {
            // Mock dependencies
            var loggerService = new Mock<LoggerService>();
            var logToS3BucketService = new Mock<ILogToS3BucketService>();
            var requestId = Guid.NewGuid().ToString();
            var mockLoggingUtility = new Mock<LoggingUtility>(loggerService.Object, logToS3BucketService.Object, requestId);

            // Set local storage configuration
            LocalFileStorageConfig.LocalDevFolder = LocalStorageTestFolder;

            // Construct file path and read file content
            string relativePath = Path.Combine(LocalStorageTestFolder, LocalResourceTest, LocalFileName);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);


            try
            {
                File.Delete(fullPath);
                Console.WriteLine("File deleted successfully.");
            }
            catch (Exception)
            {
                // Ignore exception if file does not exist
            }


            // Initialize service
            var localFileService = new LocalFileService(mockLoggingUtility.Object);

            // Perform SaveResource operation
            await localFileService.SaveResource(LocalResourceTest, LocalFileName, ExpectedFileContent);

            string fileContent = File.ReadAllText(fullPath);

            Console.WriteLine($"Captured file content: {fileContent}");

            // Assert that the content matches the expected value
            Assert.That(fileContent, Is.EqualTo(ExpectedFileContent));
        }
    }
}
