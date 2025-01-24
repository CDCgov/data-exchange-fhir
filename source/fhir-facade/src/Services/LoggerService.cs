using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using OneCDPFHIRFacade.Config;
using Serilog;
using System.Text.Json;
using DateTime = System.DateTime;


namespace OneCDPFHIRFacade
{
    public class LoggerService
    {
        public async Task LogData(string logMessage)
        {
            if (LocalFileStorageConfig.UseLocalDevFolder)
            {
                ConsoleLogs(logMessage);
            }
            else
            {
                await CloudWatchLogs(logMessage);
            }
        }
        public void ConsoleLogs(string logMessage)
        {
            var jsonLogMessage = JsonSerializer.Serialize(logMessage);

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information(jsonLogMessage);
        }
      
        public async Task CloudWatchLogs(string logMessage)
        {
            //AWS CloudWatch logs inst
            BasicAWSCredentials credentials = new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey);

            AmazonCloudWatchLogsConfig config = new AmazonCloudWatchLogsConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(AwsConfig.Region)
            };

            AmazonCloudWatchLogsClient logClient = new AmazonCloudWatchLogsClient(credentials, config);

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

                // Prepare log event
                var logEvent = new InputLogEvent
                {
                    Message = logMessage,
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
}