using Amazon.CloudWatchLogs.Model;
using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Utilities
{
    public class ServiceAvailabilityUtility
    {
        public async Task<List<string>> ServiceAvailable()
        {
            List<string> message = new List<string>();
            try
            {
                // Check if log group
                if (AwsConfig.logsClient == null)
                {
                    message.Add("Log Service is unavailable.");
                }
                else
                {
                    var describeResponse = await AwsConfig.logsClient.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
                    {
                        LogGroupName = AwsConfig.LogGroupName
                    });
                    message.Add("Log Service is available and healthy.");
                }
                //Check if S3 is available
                if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
                {
                    message.Add("S3 Bucket is unavailable.");
                }
                else
                {
                    var response = await AwsConfig.S3Client.ListBucketsAsync();
                    if (!response.Buckets.Exists(b => b.BucketName == AwsConfig.BucketName))
                    {
                        message.Add("S3 Bucket is unavailable.");
                    }

                    else
                    {
                        message.Add("S3 Bucket is available and healhy.");
                    }
                }
                return message;
            }
            catch
            {
                message.Add("Failed to get access to services.");
                return message;
            }
        }
    }
}
