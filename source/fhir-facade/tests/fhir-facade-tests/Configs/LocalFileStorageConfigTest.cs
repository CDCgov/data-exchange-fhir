using Microsoft.Extensions.Configuration;
using OneCDPFHIRFacade.Config;

namespace fhir_facade_tests.Configs
{
    internal class LocalFileStorageConfigTest
    {
        private IConfiguration CreateMockConfiguration(Dictionary<string, string?> configValues)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();
        }

        [Test]
        public void Initialize_ShouldLoadLocalDevFolder_WhenConfigured()
        {
            // Arrange
            var configValues = new Dictionary<string, string?>
        {
            { "FileSettings:LocalDevFolder", "/tmp/dev-folder" },
            { "RunEnvironment", "Local" }
        };

            var configuration = CreateMockConfiguration(configValues);

            // Act
            LocalFileStorageConfig.Initialize(configuration);

            // Assert
            Assert.That(LocalFileStorageConfig.LocalDevFolder, Is.EqualTo("/tmp/dev-folder"));
            Assert.That(LocalFileStorageConfig.UseLocalDevFolder, Is.True);
        }

        [Test]
        public void Initialize_ShouldDisableLocalDevFolder_WhenEnvironmentIsNotLocal()
        {
            // Arrange
            var configValues = new Dictionary<string, string?>
        {
            { "FileSettings:LocalDevFolder", "/tmp/dev-folder" },
            { "RunEnvironment", "Production" }
        };

            var configuration = CreateMockConfiguration(configValues);

            // Act
            LocalFileStorageConfig.Initialize(configuration);

            // Assert
            Assert.That(LocalFileStorageConfig.LocalDevFolder, Is.EqualTo("/tmp/dev-folder"));
            Assert.That(LocalFileStorageConfig.UseLocalDevFolder, Is.False);
        }

        [Test]
        public void Initialize_ShouldHandleMissingValues()
        {
            // Arrange: Empty configuration (simulating missing values)
            var configuration = CreateMockConfiguration(new Dictionary<string, string?>());

            // Act
            LocalFileStorageConfig.Initialize(configuration);

            // Assert: Values should be null when not set
            Assert.That(LocalFileStorageConfig.LocalDevFolder, Is.Null);
            Assert.That(LocalFileStorageConfig.UseLocalDevFolder, Is.False);
        }
    }
}
