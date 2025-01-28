using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

            // Register serivces, Create instances of LocalFileService and S3FileService
            builder.Services.AddSingleton<ILocalFileService, LocalFileService>();
            builder.Services.AddSingleton<IS3FileService, S3FileService>();

            // Initialize AWS configuration
            AwsConfig.Initialize(builder.Configuration);
            // Initialize Local file storage configuration
            LocalFileStorageConfig.Initialize(builder.Configuration);
            //Initailize loggerService
            LoggerService loggerService = new LoggerService();

            if (runEnvironment == "AWS")
            {
                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region), // Set region
                    ServiceURL = AwsConfig.ServiceURL                                  // Optional: Set custom service URL
                };

                // Initialize the client with credentials and config
                if (string.IsNullOrEmpty(AwsConfig.AccessKey))
                {
                    AwsConfig.S3Client = new AmazonS3Client(s3Config);
                }
                else
                {
                    AwsConfig.S3Client = new AmazonS3Client(new BasicAWSCredentials(AwsConfig.AccessKey, AwsConfig.SecretKey), s3Config);
                }
            }// .if

            if (!string.IsNullOrEmpty(AwsConfig.OltpEndpoint))
            {
                await loggerService.LogData(AwsConfig.OltpEndpoint, " ProgramOLTP ");
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
                           options.Endpoint = new Uri(AwsConfig.OltpEndpoint);
                           options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                       })
                       .AddProcessor(new SimpleActivityExportProcessor(new OpenTelemetryS3Exporter()));
               });


                builder.Services.AddOpenTelemetry().WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation() // Collect ASP.NET Core metrics
                        .AddHttpClientInstrumentation() // Collect HTTP client metrics
                        .AddMeter("OneCDPFHIRFacadeMeter").AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(AwsConfig.OltpEndpoint);
                        }).AddConsoleExporter();  // Custom metrics              
                });
            }
            else
            {
                await loggerService.LogData("No OLTP", " ProgramOLTP ");
            }

            // Configure JwtBearer authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Specify the authority and audience
                    options.Authority = AwsConfig.AuthValidateURL;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AwsConfig.AuthValidateURL,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiredScope", policy =>
                {
                    policy.RequireAssertion(async context =>
                    {
                        // Instantiate the validator with required suffixes
                        var scopeValidator = new ScopeValidator("patient/bundle.*");

                        // Get the scope claim
                        var scopeClaim = context.User.FindFirst("scope")?.Value;

                        // Validate the scopes
                        return await scopeValidator.Validate(scopeClaim);
                    });
                });
            });

            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        await loggerService.LogData($"Authentication failed: {context.Exception.Message}", "Validator");
                        //Todo: add logs to S3
                    },
                    OnTokenValidated = async context =>
                    {
                        Console.WriteLine("Token validated successfully.");
                        await loggerService.LogData("Token validated successfully.", "Validator");
                        //Todo: add logs to S3
                    },
                };
            });


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
            await app.RunAsync();

        }
    }
}
