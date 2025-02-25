using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.S3;
using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Utilities
{
    public class ServiceAvailibilityUtility
    {
        public async Task<bool> IsLogGroupAvailable()
        {
            AmazonCloudWatchLogsClient _logClient = AwsConfig.logsClient!;
            // Fetch or create the log group
            try
            {
                var describeResponse = await _logClient!.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
                {
                    LogGroupName = AwsConfig.LogGroupName
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsS3Available()
        {
            AmazonS3Client s3Client = AwsConfig.S3Client!;
            // Fetch or create the log group
            if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
            {
                return false;
            }
            return true;

        }
    }
}
