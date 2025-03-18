using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
using OpenTelemetry;
using System.Diagnostics;
using System.Text.Json;

namespace OneCDPFHIRFacade.Services
{
    public class OpenTelemetryS3Exporter : BaseExporter<Activity>
    {
        private readonly LoggingUtility _loggingUtility;
        private readonly FileServiceFactory _fileServiceFactory;

        public OpenTelemetryS3Exporter(LoggingUtility loggingUtility, FileServiceFactory fileServiceFactory)
        {
            _loggingUtility = loggingUtility ?? throw new ArgumentNullException(nameof(loggingUtility));
            _fileServiceFactory = fileServiceFactory ?? throw new ArgumentNullException(nameof(fileServiceFactory));
        }

        public override ExportResult Export(in Batch<Activity> batch)
        {
            var fileService = _fileServiceFactory.CreateFileService(AwsConfig.S3Client == null);

            using var scope = SuppressInstrumentationScope.Begin();

            // Iterate through each activity in the batch and save to the appropriate storage (S3 or local)
            foreach (var activity in batch)
            {
                Console.WriteLine($"Processing activity: {activity.Id}");


                // Serialize the activity object to JSON
                var jsonString = JsonSerializer.Serialize(activity);

                // Save the serialized JSON string asynchronously (no waiting in this context)
                fileService.SaveResource("Activity", $"{activity.Id}.json", jsonString);
            }

            return ExportResult.Success;
        }
    }
}
