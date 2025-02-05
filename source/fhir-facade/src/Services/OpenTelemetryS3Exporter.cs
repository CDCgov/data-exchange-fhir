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

        public OpenTelemetryS3Exporter(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility;
        }
        public override ExportResult Export(in Batch<Activity> batch)
        {
            using var scope = SuppressInstrumentationScope.Begin();
            S3FileService s3FileService = new S3FileService(_loggingUtility);

            // Iterate through each activity in the batch and upload to S3
            foreach (var activity in batch)
            {
                if (AwsConfig.S3Client != null)
                {
                    // Serialize the activity object to JSON
                    string jsonString = JsonSerializer.Serialize(activity);

                    // Save the serialized JSON string to S3
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    s3FileService.SaveOpenTelemetryToS3($"Activity", activity.Id + ".json", jsonString);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            return ExportResult.Success;
        }
    }
}