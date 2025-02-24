using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
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
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            //Allow files as big as 300mb
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 300 * 1024 * 1024; // 300MB limit
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 300MB limit
            });

            // Set this via config or environment
            // #####################################################
            // UseLocalDevFolder to true for Local development and Not AWS
            // UseLocalDevFolder to false will be using AWS
            // #####################################################
            string runEnvironment = builder.Configuration.GetValue<string>("RunEnvironment")!;

            // Initialize Local file storage configuration
            LocalFileStorageConfig.Initialize(builder.Configuration);
            builder.Services.AddHttpContextAccessor();

            // #####################################################
            // AWS Configuration
            // #####################################################
            if (runEnvironment == "AWS")
            {
                // Initialize AWS configuration
                AwsConfig.Initialize(builder.Configuration);
                AwsConfig.Initialize(builder.Configuration);

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
                builder.Services.AddSingleton<ILogToS3BucketService, LogToS3BucketService>();
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
                                Console.WriteLine("Client Scope not provided");
                                return false;
                            }

                            var clientScope = AwsConfig.ClientScope;

                            if (!string.IsNullOrEmpty(context.User.FindFirst("scope")?.Value))
                            {
                                // Get the scope claim
                                var scopeClaim = context.User.FindFirst("scope")?.Value;
                                AwsConfig.ScopeClaim = scopeClaim!.Split(' ');
                                UserIdFromScopeUtility userIdFromScope = new UserIdFromScopeUtility();
                                userIdFromScope.GetUserIdFromScope();
                                // Validate the scopes claim from JWT token are scopes from config
                                // checks sent scopes are onboarded scopes in config
                                return await scopeValidator.Validate(scopeClaim);
                            }
                            else
                            {
                                Console.WriteLine("Scope claim not provided");
                                AwsConfig.ScopeClaim = [];
                                return false;
                            }
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
                            await loggingUtility.Logging($"Authentication failed: {context.Exception.Message}");
                        },
                        OnTokenValidated = async context =>
                        {
                            Console.WriteLine("Token validated successfully.");

                            var loggingUtility = context.HttpContext.RequestServices.GetRequiredService<LoggingUtility>();
                            await loggingUtility.Logging("Token validated successfully.");
                        },
                    };
                });
                builder.Services.AddSingleton<ScopeValidator>();
                builder.Services.AddScoped<LoggingUtility>(sp =>
                {
                    var loggerService = sp.GetRequiredService<LoggerService>();
                    var logToS3BucketService = sp.GetRequiredService<ILogToS3BucketService>();
                    var httpContext = sp.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

                    var requestId = httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

                    return new LoggingUtility(loggerService, logToS3BucketService, requestId);
                });

            }// .if
            // #####################################################
            // Local - Non-AWS Configuration
            // #####################################################
            else
            {
                builder.Services.AddSingleton(new LoggerService());
                builder.Services.AddScoped<LoggingUtility>(sp =>
                {
                    var loggerService = sp.GetRequiredService<LoggerService>();
                    var httpContext = sp.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

                    var requestId = httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

                    return new LoggingUtility(loggerService, requestId);
                });
            }
            // #####################################################
            // ./ end Configuration
            // #####################################################

            // #####################################################
            // Register serivces, Create instances of Logging
            // #####################################################
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
            // #####################################################
            // ./ end - Register serivces, Create instances of Logging
            // #####################################################

            // #####################################################
            // Now Build the App
            // #####################################################
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

            if (runEnvironment == "AWS")
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.MapControllers();
            // #####################################################
            // Start the App
            // #####################################################
            await app.RunAsync();
        }// /. public static async Task Main(string[] args)
    }// ./ public static class Program
}// ./ namespace OneCDPFHIRFacade