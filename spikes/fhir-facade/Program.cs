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
            var useLocalDevFolder = builder.Configuration.GetValue<bool>("UseLocalDevFolder");
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // #####################################################
            // POST endpoint for Bundle
            // #####################################################
            //app.MapPost("/Bundle", async (HttpContext httpContext, ILocalFileService localFileService, IS3FileService s3FileService) =>
            //{

            //    var parser = new FhirJsonParser();
            //    Bundle bundle;

            //    try
            //    {
            //        // Read the request body as a string
            //        var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
            //        // Parse JSON string to FHIR Bundle object
            //        bundle = parser.Parse<Bundle>(requestBody);

            //        // Check if Patient ID is present
            //        if (string.IsNullOrWhiteSpace(bundle.Id))
            //        {
            //            return Results.BadRequest(new
            //            {
            //                error = "Invalid payload",
            //                message = "Resource ID is required."
            //            });
            //        }

            //        // Log details to console
            //        Console.WriteLine($"Received FHIR Bundle: Id={bundle.Id}");

            //        // Generate a new UUID for the file name
            //        var fileName = $"{Guid.NewGuid()}.json";
            //        var resourceJson = bundle.ToJson();

            //        if (useLocalDevFolder)
            //        {
            //            // #####################################################
            //            // Save the FHIR Resource Locally
            //            // #####################################################
            //            return await localFileService.SaveResourceLocally(localReceivedFolder, "Bundle", fileName, resourceJson);

            //        } // .if UseLocalDevFolder
            //        else
            //        {
            //            // #####################################################
            //            // Save the FHIR Resource to AWS S3
            //            // #####################################################
            //            if (AwsConfig.S3Client == null || string.IsNullOrEmpty(AwsConfig.BucketName))
            //            {
            //                return Results.Problem("S3 client and bucket are not configured.");
            //            }
            //            return await s3FileService.SaveResourceToS3(AwsConfig.S3Client, AwsConfig.BucketName, "Bundle", fileName, resourceJson);
            //        }// .else
            //    }
            //    catch (FormatException ex)
            //    {
            //        // Return 400 Bad Request if JSON is invalid
            //        return Results.BadRequest(new
            //        {
            //            error = "Invalid payload",
            //            message = $"Failed to parse FHIR Resource: {ex.Message}"
            //        });
            //    }
            //});

            app.MapControllers();
            // #####################################################
            // Start the App
            // #####################################################
            app.Run();

        }
    }
}
