using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using OneCDPFHIRFacade.Config;
using Serilog;
using System.Text.Json;

namespace OneCDP.Logging
{
    public class LoggerService
    {
        public readonly AmazonCloudWatchLogsClient? _logClient;
        public readonly string? _logGroupName;

        public LoggerService(AmazonCloudWatchLogsClient logsClient, string logGroupName)
        {
            this._logClient = logsClient;
            this._logGroupName = logGroupName;
        }

        public LoggerService()
        {
            _logClient = null;
            _logGroupName = null;
        }
        public async Task LogData(string message, string requestId, bool env)
        {
            if (env)
            {
                ConsoleLogs(message, requestId);
            }
            else
            {
                await CloudWatchLogs(message, requestId);
            }
        }

        public void ConsoleLogs(string message, string requestId)
        {
            var logMessage = new
            {
                RequestID = requestId,
                ClientID = AwsConfig.ClientId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };
            var jsonLogMessage = JsonSerializer.Serialize(logMessage);

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information(jsonLogMessage);
        }

        public async Task CloudWatchLogs(string message, string requestId)
        {
            try
            {
                var logStreamName = $"{DateTime.UtcNow:yyyyMMdd}";

                // Fetch or create the log stream
                var describeResponse = await _logClient!.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
                {
                    LogGroupName = _logGroupName,
                    LogStreamNamePrefix = logStreamName
                });

                var logStream = describeResponse.LogStreams.FirstOrDefault(ls => ls.LogStreamName == logStreamName);
                string? sequenceToken = null;

                if (logStream == null)
                {
                    await _logClient.CreateLogStreamAsync(new CreateLogStreamRequest(_logGroupName, logStreamName));
                }
                else
                {
                    sequenceToken = logStream.UploadSequenceToken;
                }

                var logEvent = new InputLogEvent
                {
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                var putLogEventsRequest = new PutLogEventsRequest
                {
                    LogGroupName = _logGroupName,
                    LogStreamName = logStreamName,
                    LogEvents = new List<InputLogEvent> { logEvent },
                    SequenceToken = sequenceToken
                };

                await _logClient.PutLogEventsAsync(putLogEventsRequest);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error appending log: " + ex.Message);
            }
        }
    }
}