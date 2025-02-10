using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OneCDP.Logging;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Services;
using OneCDPFHIRFacade.Utilities;
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
                builder.Services.AddSingleton(new LoggerService(AwsConfig.logsClient!, AwsConfig.LogGroupName!));
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
                            var httpContext = context.Resource as HttpContext;
                            if (httpContext == null)
                            {
                                Console.WriteLine("Authentication URL not provided");
                                return false;
                            }
                            // Instantiate the validator with required scope
                            var scopeValidator = httpContext.RequestServices.GetRequiredService<ScopeValidator>();
                            if (AwsConfig.ClientScope.IsNullOrEmpty())
                            {
                                Console.WriteLine("Scope not provided");
                                return false;
                            }

                            var clientScope = AwsConfig.ClientScope;

                            // Get the scope claim
                            var scopeClaim = context.User.FindFirst("scope")?.Value;

                            // Validate the scopes claim from JWT token are scopes from config
                            // checks sent scopes are onboarded scopes in config
                            return await scopeValidator.Validate(scopeClaim, clientScope!);
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

                            var loggingUtility = context.HttpContext.RequestServices.GetRequiredService<LoggingUtility>();
                            await loggingUtility.Logging($"Authentication failed: {context.Exception.Message}", "Validator");
                        },
                        OnTokenValidated = async context =>
                        {
                            Console.WriteLine("Token validated successfully.");

                            var loggingUtility = context.HttpContext.RequestServices.GetRequiredService<LoggingUtility>();
                            await loggingUtility.Logging("Token validated successfully.", "Validator");
                        },
                    };
                });
                builder.Services.AddSingleton<ScopeValidator>();
            }// .if

            else
            {
                builder.Services.AddSingleton(new LoggerService());
            }
            // Register serivces, Create instances of Logging
            builder.Services.AddSingleton<ILogToS3BucketService, LogToS3BucketService>();
            builder.Services.AddSingleton<LoggingUtility>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var loggerService = scope.ServiceProvider.GetRequiredService<LoggerService>();
                var loggingUtility = scope.ServiceProvider.GetRequiredService<LoggingUtility>();
                if (!string.IsNullOrEmpty(AwsConfig.OltpEndpoint))
                {
                    bool runLocal = LocalFileStorageConfig.UseLocalDevFolder;
                    await loggerService.LogData(AwsConfig.OltpEndpoint, " ProgramOLTP ", runLocal);

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
                           .AddProcessor(new SimpleActivityExportProcessor(new OpenTelemetryS3Exporter(loggingUtility)));
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
                    await loggingUtility.Logging("No OLTP", " ProgramOLTP ");
                }


            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            if (!LocalFileStorageConfig.UseLocalDevFolder)
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.MapControllers();
            // #####################################################
            // Start the App
            // #####################################################
            await app.RunAsync();
        }
    }
}
