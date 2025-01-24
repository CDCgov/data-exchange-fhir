using OneCDPFHIRFacade.Services;
using System.Text.Json;

namespace OneCDPFHIRFacade.Utilities
{
    public class LoggingUtility
    {
        private readonly List<Object> resultList = new List<Object>();

        public async Task Logging(string message, string requestId)
        {
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };

            await CloudWatchLogger(logMessage);
            AddLogForS3(logMessage);
        }

        public static async Task CloudWatchLogger(Object logMessage)
        {
            string jsonString = JsonSerializer.Serialize(logMessage);
            LoggerService loggerService = new LoggerService();
            await loggerService.LogData(jsonString);
        }
        public async Task SaveLogS3(string requestId)
        {
            var jsonLogMessage = JsonSerializer.Serialize(resultList);
            LogToS3FileService logToS3FileService = new LogToS3FileService();
            await logToS3FileService.SaveResourceToS3(jsonLogMessage, requestId);
        }

        public void AddLogForS3(Object logMessage)
        {
            resultList.Add(logMessage);
        }
    }
}
