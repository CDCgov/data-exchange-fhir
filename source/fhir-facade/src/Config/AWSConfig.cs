using Amazon.CloudWatchLogs;
using Amazon.S3;

namespace OneCDPFHIRFacade.Config
{
    public static class AwsConfig
    {
        public const string KeyName = "AWS";
        public static string? Region { get; private set; }
        public static string? ServiceURL { get; private set; }
        public static string? AccessKey { get; private set; }
        public static string? SecretKey { get; set; }
        public static string? BucketName { get; set; }
        public static string? LogGroupName { get; set; }
        public static string? OltpEndpoint { get; set; }
        public static string? AuthValidateURL { get; set; }
        public static string[]? ClientScope { get; set; }
        public static string[]? ScopeClaim { get; set; }
        public static string? ClientId { get; set; }
        public static AmazonS3Client? S3Client { get; set; }
        public static AmazonCloudWatchLogsClient? logsClient { get; set; }



        public static void Initialize(IConfiguration configuration)
        {
            var section = configuration.GetSection(KeyName);
            Region = section.GetValue<string>("Region");
            ServiceURL = section.GetValue<string>("ServiceURL");
            AccessKey = section.GetValue<string>("AccessKey");
            SecretKey = section.GetValue<string>("SecretKey");
            BucketName = section.GetValue<string>("BucketName");
            OltpEndpoint = section.GetValue<string>("OltpEndpoint");
            LogGroupName = section.GetValue<string>("LogGroupName");
            AuthValidateURL = section.GetValue<string>("VerifyAuthURL");
            var clientScopeList = new List<string>();
            section.GetSection("ClientScope").Bind(clientScopeList);
            ClientScope = clientScopeList.ToArray();
        }
    }
}
