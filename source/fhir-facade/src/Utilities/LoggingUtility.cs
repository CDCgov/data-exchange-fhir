using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using System.Text.Json;


namespace OneCDPFHIRFacade.Utilities
{
    public class LoggingUtility
    {
        private readonly List<string> _resultList = new List<string>();
        private readonly LoggerService _loggerService;
        private readonly ILogToS3BucketService _logToS3FileService;

        // Inject the dependencies via constructor
        public LoggingUtility(LoggerService loggerService, ILogToS3BucketService logToS3FileService)
        {
            _loggerService = loggerService;
            _logToS3FileService = logToS3FileService;
        }
        public async Task Logging(string message, string requestId)
        {
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };
            string jsonString = JsonSerializer.Serialize(logMessage);

            await _loggerService.LogData(jsonString, requestId);
            AddLogForS3(jsonString);
        }
        public async Task SaveLogS3(string bucketName, string fileName, string requestId)
        {
            // Serialize the logs for S3
            var jsonLogMessage = JsonSerializer.Serialize(_resultList);

            // Save logs to S3
            await _logToS3FileService.SaveResourceToS3(AwsConfig.S3Client!, bucketName, fileName, requestId);
        }
        public void AddLogForS3(string logMessage)
        {
            _resultList.Add(logMessage);
        }
    }
}

