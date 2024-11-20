using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace OneCDPFHIRFacade.Configs
{
    public class AWSConfig
    {
        public static string KeyName = "AWS";
        public static string Region { get; set; } = "";
        public static string ServiceURL { get; set; } = "";
        public static string AccessKey { get; set; } = "";
        public static string SecretKey { get; set; } = "";
        public static string BucketName { get; set; } = "";

        public static AmazonS3Client? S3Client { get; set; }

        public AWSConfig()
        {
            var useLocalDevFolder = FileStorageConfig.UseLocalDevFolder;
            var useAWSS3 = !useLocalDevFolder;

            if (useAWSS3)
            {

                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(Region), // Set region    
                };

                // Initialize the client with credentials and config
                S3Client = new AmazonS3Client(new BasicAWSCredentials(AccessKey, SecretKey), s3Config);
            }
        }
    }


}
