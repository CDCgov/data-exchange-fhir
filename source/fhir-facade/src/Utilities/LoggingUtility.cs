using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using System.Text.Json;


namespace OneCDPFHIRFacade.Utilities
{
    public class LoggingUtility
    {
        private readonly bool runEnv = LocalFileStorageConfig.UseLocalDevFolder;
        // Inject the dependencies via constructor
        private readonly LoggerService _loggerService;
        private readonly ILogToS3BucketService? _logToS3BucketService;
        public string requestId;

        public LoggingUtility(LoggerService loggerService, ILogToS3BucketService logToS3BucketService, string requestId)
        {
            _loggerService = loggerService;
            _logToS3BucketService = logToS3BucketService;
            this.requestId = requestId;
        }
        public LoggingUtility(LoggerService loggerService, string requestId)
        {
            _loggerService = loggerService;
            this.requestId = requestId;
        }
        public virtual async Task Logging(string message)
        {
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };
            string jsonString = JsonSerializer.Serialize(logMessage);

            await _loggerService.LogData(jsonString, requestId, runEnv);
            if (!runEnv)
            {
                AddLogForS3(logMessage);
            }
        }
        public async Task SaveLogS3(string fileName)
        {
            // Save logs to S3
            await _logToS3BucketService!.SaveResourceToS3(AwsConfig.S3Client!, AwsConfig.BucketName!, fileName);
        }
        public void AddLogForS3(object logMessage)
        {
            _logToS3BucketService!.JsonResult(logMessage);
        }
    }
}