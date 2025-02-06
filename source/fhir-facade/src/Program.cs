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
            }// .if

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<LoggingUtility>(sp =>
            {
                var loggerService = sp.GetRequiredService<LoggerService>();
                var logToS3BucketService = sp.GetRequiredService<ILogToS3BucketService>();
                var httpContext = sp.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

                var requestId = httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

                return new LoggingUtility(loggerService, logToS3BucketService, requestId);
            });

            builder.Services.AddSingleton<ILogToS3BucketService, LogToS3BucketService>();
            builder.Services.AddScoped<ScopeValidator>();
            // Register serivces, Create instances of Logging
            builder.Services.AddSingleton<LoggerService>(sp =>
                new LoggerService(AwsConfig.logsClient!, AwsConfig.LogGroupName!)
            );

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
                        //Create request ID
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

                        string requestId = httpContext.TraceIdentifier;
                        Console.WriteLine($"[{requestId}] Validating scopes...");

                        var clientScope = AwsConfig.ClientScope;
                        var scopeClaim = context.User.FindFirst("scope")?.Value;

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
                        var loggingUtility = context.HttpContext.RequestServices.GetRequiredService<LoggingUtility>();
                        string requestId = context.HttpContext.TraceIdentifier;

                        Console.WriteLine($"[{requestId}] Authentication failed: {context.Exception.Message}");
                        await loggingUtility.Logging($"[{requestId}] Authentication failed: {context.Exception.Message}");
                    },
                    OnTokenValidated = async context =>
                    {
                        var loggingUtility = context.HttpContext.RequestServices.GetRequiredService<LoggingUtility>();
                        string requestId = context.HttpContext.TraceIdentifier; // Ensure request ID consistency

                        Console.WriteLine($"[{requestId}] Token validated successfully.");
                        await loggingUtility.Logging($"[{requestId}] Token validated successfully.");
                    },
                };
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
                            using var scope = sp.CreateScope();
                            var loggingUtility = scope.ServiceProvider.GetRequiredService<LoggingUtility>();
                            return new SimpleActivityExportProcessor(new OpenTelemetryS3Exporter(loggingUtility));
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
                            options.Endpoint = new Uri(AwsConfig.OltpEndpoint);
                        }).AddConsoleExporter();  // Custom metrics              
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
                    loggerService.LogData(AwsConfig.OltpEndpoint, "OLTP", runLocal)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    await loggingUtility.Logging("No OLTP");
                }
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