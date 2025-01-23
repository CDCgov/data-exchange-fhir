using OneCDPFHIRFacade.Services;
using System.Text.Json;

namespace OneCDPFHIRFacade.Utilities
{
    public class LoggingUtility
    {
        public List<Object> resultList = new List<Object>();

        public void Logging(string message, string requestId)
        {
            //Log message as json
            var logMessage = new
            {
                RequestID = requestId,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };

            CloudWatchLogger(logMessage);
            AddLogForS3(logMessage);
        }

        public async void CloudWatchLogger(Object logMessage)
        {
            string jsonString = JsonSerializer.Serialize(logMessage);
            LoggerService loggerService = new LoggerService();
            await loggerService.LogData(jsonString);
        }
        public async void SaveLogS3(string requestId)
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
