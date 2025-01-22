using OneCDPFHIRFacade.Config;
using OpenTelemetry;
using System.Diagnostics;
using System.Text.Json;

namespace OneCDPFHIRFacade.Services
{
    public class OpenTelemetryS3Exporter : BaseExporter<Activity>
    {
        private readonly string name = "OpenTelemetryS3Exporter";

        public override ExportResult Export(in Batch<Activity> batch)
        {
            using var scope = SuppressInstrumentationScope.Begin();

            LoggerService logEntry = new LoggerService();
            LogToS3FileService logToS3FileService = new LogToS3FileService();
            S3FileService s3FileService = new S3FileService(logToS3FileService);

            // Iterate through each activity in the batch and upload to S3
            foreach (var activity in batch)
            {
                if (AwsConfig.S3Client != null)
                {
                    // Serialize the activity object to JSON
                    string jsonString = JsonSerializer.Serialize(activity);

                    // Save the serialized JSON string to S3
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    s3FileService.SaveResourceToS3(AwsConfig.S3Client, AwsConfig.BucketName!, "Activity", activity.Id + ".json", jsonString, logEntry, activity.Id);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }

            return ExportResult.Success;
        }
    }
}
