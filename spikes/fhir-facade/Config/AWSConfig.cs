using Amazon.S3;

namespace OneCDPFHIRFacade.Config
{
    public static class AwsConfig
    {
        public const string KeyName = "AWS";
        public static string? Region { get; private set; }
        public static string? ServiceURL { get; private set; }
        public static string? BucketName { get; set; }
        public static string? LogGroupName { get; set; }
        public static string? OltpEndpoint { get; set; }
        public static AmazonS3Client? S3Client { get; set; }


        public static void Initialize(IConfiguration configuration)
        {
            var section = configuration.GetSection(KeyName);
            Region = section.GetValue<string>("Region");
            ServiceURL = section.GetValue<string>("ServiceURL");
            BucketName = section.GetValue<string>("BucketName");
            OltpEndpoint = section.GetValue<string>("OltpEndpoint");
            LogGroupName = section.GetValue<string>("LogGroupName");
        }
    }
}
