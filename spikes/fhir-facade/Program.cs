using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OneCDPFHIRFacade
{
    public static class Program
    {
        public static void Main(string[] args)
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
            var useLocalDevFolder = LocalFileStorageConfig.UseLocalDevFolder;
            var useAWSS3 = !useLocalDevFolder;

            // Register serivces, Create instances of LocalFileService and S3FileService
            builder.Services.AddSingleton<ILocalFileService, LocalFileService>();
            builder.Services.AddSingleton<IS3FileService, S3FileService>();
            // TODO
            // builder.Services.AddSingleton(useLocalDevFolder);
            // builder.Services.AddSingleton(new AwsConfig());

            // Initialize AWS configuration
            AwsConfig.Initialize(builder.Configuration);
            // Initialize Local file storage configuration
            LocalFileStorageConfig.Initialize(builder.Configuration);

            if (useAWSS3)
            {
                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region), // Set region
                    ServiceURL = AwsConfig.ServiceURL                                  // Optional: Set custom service URL
                };

                // Initialize the client with credentials and config
                AwsConfig.S3Client = new AmazonS3Client(new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey), s3Config);
            }// .if

            //CloudWatchLoggerService logEntry = new CloudWatchLoggerService();

            if (!string.IsNullOrEmpty(AwsConfig.OltpEndpoint))
            {
                //logEntry.AppendLogAsync(AwsConfig.OltpEndpoint, " ProgramOLTP ");
                builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
               {
                   tracerProviderBuilder
                       .SetResourceBuilder(resourceBuilder)
                       .AddAspNetCoreInstrumentation() // Instruments ASP.NET Core (HTTP request handling)
                       .AddHttpClientInstrumentation() // Instruments outgoing HTTP client requests
                       .AddAWSInstrumentation()
                       .AddConsoleExporter()
                       .AddOtlpExporter(options =>
                       {
                           options.Endpoint = new Uri(AwsConfig.OltpEndpoint); // "http://localhost:4317");
                           options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                       });
               });


                builder.Services.AddOpenTelemetry().WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation() // Collect ASP.NET Core metrics
                        .AddHttpClientInstrumentation() // Collect HTTP client metrics
                        .AddMeter("OneCDPFHIRFacadeMeter").AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(AwsConfig.OltpEndpoint); // "http://localhost:4317");                   
                        }).AddConsoleExporter();  // Custom metrics              
                });
            }
            else
            {
                //logEntry.AppendLogAsync("No OLTP", " ProgramOLTP ");
            }

            var app = builder.Build();

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
            app.Run();

        }
    }
}
