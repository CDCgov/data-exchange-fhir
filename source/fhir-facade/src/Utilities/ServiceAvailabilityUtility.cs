using Amazon.CloudWatchLogs.Model;
using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Utilities
{
    public class ServiceAvailabilityUtility
    {
        public async Task<bool> IsServiceAvailable()
        {
            try
            {
                // Fetch or create the log group
                if (AwsConfig.logsClient == null)
                {
                    return false;
                }
                var describeResponse = await AwsConfig.logsClient.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
                {
                    LogGroupName = AwsConfig.LogGroupName
                });
                //Check if S3 is available
                if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
                {
                    return false;
                }
                var response = await AwsConfig.S3Client.ListBucketsAsync();
                if (!response.Buckets.Exists(b => b.BucketName == AwsConfig.BucketName))
                {
                    return false;
                }
                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
