using Microsoft.Extensions.Configuration;
using OneCDPFHIRFacade.Config;

namespace fhir_facade_tests.Configs
{
    internal class AwsConfigTest
    {
        private IConfiguration CreateMockConfiguration(Dictionary<string, string?> configValues)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();
        }

        [Test]
        public void Initialize_ShouldLoadConfigurationValues()
        {
            // Arrange
            var configValues = new Dictionary<string, string?>
        {
            { "AWS:Region", "us-east-1" },
            { "AWS:ServiceURL", "https://s3.amazonaws.com" },
            { "AWS:AccessKey", "test-access-key" },
            { "AWS:SecretKey", "test-secret-key" },
            { "AWS:BucketName", "my-test-bucket" },
            { "AWS:OltpEndpoint", "https://oltp.endpoint.com" },
            { "AWS:LogGroupName", "my-log-group" },
            { "AWS:VerifyAuthURL", "https://auth.validate.com" },
            { "AWS:ClientScope:0", "scope1" },
            { "AWS:ClientScope:1", "scope2" }
        };

            var configuration = CreateMockConfiguration(configValues);

            // Act
            AwsConfig.Initialize(configuration);

            // Assert
            Assert.That(AwsConfig.Region, Is.EqualTo("us-east-1"));
            Assert.That(AwsConfig.ServiceURL, Is.EqualTo("https://s3.amazonaws.com"));
            Assert.That(AwsConfig.AccessKey, Is.EqualTo("test-access-key"));
            Assert.That(AwsConfig.SecretKey, Is.EqualTo("test-secret-key"));
            Assert.That(AwsConfig.BucketName, Is.EqualTo("my-test-bucket"));
            Assert.That(AwsConfig.OltpEndpoint, Is.EqualTo("https://oltp.endpoint.com"));
            Assert.That(AwsConfig.LogGroupName, Is.EqualTo("my-log-group"));
            Assert.That(AwsConfig.AuthValidateURL, Is.EqualTo("https://auth.validate.com"));
            Assert.That(AwsConfig.ClientScope, Is.Not.Null);
            Assert.That(AwsConfig.ClientScope, Is.EqualTo(new[] { "scope1", "scope2" }));
        }

        [Test]
        public void Initialize_ShouldHandleMissingValues()
        {
            // Arrange: Empty configuration (simulating missing values)
            var configuration = CreateMockConfiguration(new Dictionary<string, string?>());

            // Act
            AwsConfig.Initialize(configuration);

            // Assert: Values should be null when not set
            Assert.That(AwsConfig.Region, Is.Null);
            Assert.That(AwsConfig.ServiceURL, Is.Null);
            Assert.That(AwsConfig.AccessKey, Is.Null);
            Assert.That(AwsConfig.SecretKey, Is.Null);
            Assert.That(AwsConfig.BucketName, Is.Null);
            Assert.That(AwsConfig.OltpEndpoint, Is.Null);
            Assert.That(AwsConfig.LogGroupName, Is.Null);
            Assert.That(AwsConfig.AuthValidateURL, Is.Null);
            Assert.That(AwsConfig.ClientScope, Is.Empty);
        }
    }
}
