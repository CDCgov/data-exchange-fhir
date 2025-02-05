using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon.S3;
using OneCDP.Logging;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.RegularExpressions;

namespace OneCDPFHIRFacade
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService("OneCDPFHIRFacade");

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            // Set this via config or environment
            // #####################################################
            // UseLocalDevFolder to true for Local development and Not AWS
            // UseLocalDevFolder to false will be using AWS
            // #####################################################
            string runEnvironment = builder.Configuration.GetValue<string>("RunEnvironment")!;

            // Initialize AWS configuration
            AwsConfig.Initialize(builder.Configuration);

            // Initialize Local file storage configuration
            LocalFileStorageConfig.Initialize(builder.Configuration);

            var requestId = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "", RegexOptions.NonBacktracking);

            if (runEnvironment == "AWS")
            {
                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region), // Set region
                    ServiceURL = AwsConfig.ServiceURL                                  // Optional: Set custom service URL
                };

                var logClient = new AmazonCloudWatchLogsConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                };

                // Initialize the client with credentials and config
                if (string.IsNullOrEmpty(AwsConfig.AccessKey))
                {
                    AwsConfig.S3Client = new AmazonS3Client(s3Config);
                    AwsConfig.logsClient = new AmazonCloudWatchLogsClient(logClient);
                }
                else
                {
                    var basicCred = new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey);
                    AwsConfig.S3Client = new AmazonS3Client(basicCred, s3Config);
                    AwsConfig.logsClient = new AmazonCloudWatchLogsClient(basicCred, logClient);
                }
            }// .if

            // Register serivces, Create instances of Logging
            builder.Services.AddSingleton<LoggerService>(sp =>
                new LoggerService(AwsConfig.logsClient!, AwsConfig.LogGroupName!)
            );

            builder.Services.AddSingleton<ILogToS3BucketService, LogToS3BucketService>();

            // Register LoggingUtility properly, ensuring dependencies are injected
            builder.Services.AddSingleton<LoggingUtility>(sp =>
            {
                var loggerService = sp.GetRequiredService<LoggerService>();
                var logToS3BucketService = sp.GetRequiredService<ILogToS3BucketService>();

                return new LoggingUtility(loggerService, logToS3BucketService, requestId);
            });
            if (!string.IsNullOrEmpty(AwsConfig.OltpEndpoint))
            {
                builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddAWSInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(AwsConfig.OltpEndpoint);
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        })
                        .AddProcessor(sp =>
                        {
                            var loggingUtility = sp.GetRequiredService<LoggingUtility>();
                            return new SimpleActivityExportProcessor(new OpenTelemetryS3Exporter(loggingUtility));
                        });
                });

                builder.Services.AddOpenTelemetry().WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddMeter("OneCDPFHIRFacadeMeter")
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(AwsConfig.OltpEndpoint);
                        })
                        .AddConsoleExporter();
                });
            }

            // Now Build the App
            var app = builder.Build();

            // Now Resolve Services & Call Methods
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerService = services.GetRequiredService<LoggerService>();
                var loggingUtility = services.GetRequiredService<LoggingUtility>();

                if (!string.IsNullOrEmpty(AwsConfig.OltpEndpoint))
                {
                    bool runLocal = LocalFileStorageConfig.UseLocalDevFolder;

                    // Call LogData synchronously
                    loggerService.LogData(AwsConfig.OltpEndpoint, requestId, runLocal)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    // Log message synchronously
                    loggingUtility.Logging("No OLTP").GetAwaiter().GetResult();
                }

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.MapControllers();
                // #####################################################
                // Start the App
                // #####################################################
                await app.RunAsync();

            }
        }
    }
}