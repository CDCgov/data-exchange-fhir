using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using OneCDPFHIRFacade.Configs;

namespace OneCDPFHIRFacade.Handlers
{
    public class AWSHandler
    {
        public AmazonS3Client AWSs3()
        {
            var useLocalDevFolder = FileStorageConfig.UseLocalDevFolder;
            var useAWSS3 = !useLocalDevFolder;
            var s3Client = AWSConfig.S3Client;

            if (useAWSS3)
            {

                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AWSConfig.Region), // Set region    
                };

                // Initialize the client with credentials and config
                s3Client = new AmazonS3Client(new BasicAWSCredentials(AWSConfig.AccessKey, AWSConfig.SecretKey), s3Config);
            }
            return s3Client;
        }

    }
}
