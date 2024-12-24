using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;

namespace OneCDPFHIRFacade
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            //Instrument logging to add context and set format+
            //LoggingInstrumentor().instrument(set_logging_format = True);

            // Log an informational message with additional context
            //logging.info("Application started", "app_version: 1.2.3, env: production");
            var builder = WebApplication.CreateBuilder(args);

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

            if (runEnvironment == "AWS")
            {
                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region), // Set region
                    ServiceURL = AwsConfig.ServiceURL                                  // Optional: Set custom service URL
                };

                // Initialize the client with credentials and config
                AwsConfig.S3Client = new AmazonS3Client(new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey), s3Config);
            }// .if

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
