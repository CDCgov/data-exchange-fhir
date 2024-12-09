using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using OneCDPFHIRFacade.Config;
using System.Text.Json;

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

    public async Task AppendLogAsync(string message, string requestId)
    {
        try
        {
            //Bundle log groups name
            var logGroupName = AwsConfig.LogGroupName;
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
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };
            var jsonLogMessage = JsonSerializer.Serialize(logMessage);

            // Prepare log event
            var logEvent = new InputLogEvent
            {
                Message = jsonLogMessage,
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

            await logClient.PutLogEventsAsync(putLogEventsRequest);
            Console.WriteLine("Log event appended successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error appending log: " + ex.Message);
        }
    }
}
