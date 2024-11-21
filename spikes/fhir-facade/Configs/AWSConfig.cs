using Amazon.S3;

namespace OneCDPFHIRFacade.Configs
{
    public class AWSConfig
    {
        public static string KeyName = "AWS";
        public static string Region { get; set; }
        public static string ServiceURL { get; set; }
        public static string AccessKey { get; set; }
        public static string SecretKey { get; set; }
        public static string BucketName { get; set; }

        public static AmazonS3Client? S3Client { get; set; }

    }


}
