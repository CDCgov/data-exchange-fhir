using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using OneCDPFHIRFacade.Config;

public class CloudWatchLoggerService
{
    private readonly BasicAWSCredentials credentials;
    private readonly AmazonCloudWatchLogsConfig config;
    private readonly AmazonCloudWatchLogsClient logClient;
    public CloudWatchLoggerService()
    {
        new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey);

        //AWS CloudWatch logs instance
        credentials = new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey);

        config = new AmazonCloudWatchLogsConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(AwsConfig.Region)
        };

        logClient = new AmazonCloudWatchLogsClient(credentials, config);
    }

    public async Task AppendLogAsync(string message)
    {
        try
        {
            //Name of Bundle log groups
            var logGroupName = "/aws/bundle-logs/";
            var logStreamName = $"{DateTime.UtcNow.ToString("yyyyMMdd")}";

            //Get the sequence token
            var describeResponse = await logClient.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
            {
                LogGroupName = logGroupName,
                LogStreamNamePrefix = logStreamName
            });

            var logStream = describeResponse.LogStreams.FirstOrDefault(ls => ls.LogStreamName == logStreamName);

            if (logStream == null)
            {
                //Add to a logs group
                await logClient.CreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));
                return;
            }

            var sequenceToken = logStream.UploadSequenceToken;

            // Prepare log event
            var logEvent = new InputLogEvent
            {
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            // Write log event
            var putLogEventsRequest = new PutLogEventsRequest
            {
                LogGroupName = logGroupName,
                LogStreamName = logStreamName,
                LogEvents = new List<InputLogEvent> { logEvent },
                SequenceToken = sequenceToken // Include the sequence token
            };

            var response = await logClient.PutLogEventsAsync(putLogEventsRequest);
            Console.WriteLine("Log event appended successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error appending log: " + ex.Message);
        }
    }
}
