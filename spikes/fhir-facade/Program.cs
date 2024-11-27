using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace OneODPFHIRFacade
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

            // Create instances of LocalFileService and S3FileService
            var localFileService = new LocalFileService();
            var s3FileService = new S3FileService();

            // Set this via config or environment
            // #####################################################
            // UseLocalDevFolder to true for Local development and Not AWS
            // UseLocalDevFolder to false will be using AWS
            // #####################################################
            var useLocalDevFolder = builder.Configuration.GetValue<bool>("UseLocalDevFolder");
            var useAWSS3 = !useLocalDevFolder;

            IAmazonS3? s3Client = null; // Declare s3Client as nullable
            String? s3BucketName = null;

            if (useAWSS3)
            {
                var awsSettings = builder.Configuration.GetSection("AWS");
                var region = awsSettings["Region"];
                var serviceUrl = awsSettings["ServiceURL"];
                var accessKey = awsSettings["AccessKey"];
                var secretKey = awsSettings["SecretKey"];

                s3BucketName = awsSettings["BucketName"];

                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region), // Set region
                    ServiceURL = serviceUrl                                  // Optional: Set custom service URL
                };

                // Initialize the client with credentials and config
                s3Client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), s3Config);
            }// .if

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var localReceivedFolder = "LocalReceivedResources";

            // #####################################################
            // POST endpoint for Patient
            // #####################################################
            app.MapPost("/Patient", async (HttpContext httpContext) =>
            {
                // Use FhirJsonParser to parse incoming JSON as FHIR Patient
                var parser = new FhirJsonParser();
                Patient patient;

                try
                {
                    // Read the request body as a string
                    var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                    // Parse JSON string to FHIR Patient object
                    patient = parser.Parse<Patient>(requestBody);
                }
                catch (FormatException ex)
                {
                    // Return 400 Bad Request if JSON is invalid
                    return Results.BadRequest(new
                    {
                        error = "Invalid payload",
                        message = $"Failed to parse FHIR Resource: {ex.Message}"
                    });
                }

                // Check if Patient ID is present
                if (string.IsNullOrWhiteSpace(patient.Id))
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid payload",
                        message = "Resource ID is required."
                    });
                }

                // Log patient details to console
                Console.WriteLine($"Received FHIR Patient: Id={patient.Id}");

                // Generate a new UUID for the file name
                // Not using the patient.id: var filePath = Path.Combine(directoryPath, $"{patient.Id}.json");
                var fileName = $"{Guid.NewGuid()}.json";
                var resourceJson = patient.ToJson();

                if (useLocalDevFolder)
                {
                    // #####################################################
                    // Save the FHIR Resource Locally
                    // #####################################################
                    return await localFileService.SaveResourceLocally(localReceivedFolder, "Patient", fileName, resourceJson);

                } // .if UseLocalDevFolder
                else
                {
                    // #####################################################
                    // Save the FHIR Resource to AWS S3
                    // #####################################################
                    if (s3Client == null || string.IsNullOrEmpty(s3BucketName))
                    {
                        return Results.Problem("S3 client and bucket are not configured.");
                    }

                    return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Patient", fileName, resourceJson);
                }// .else

            })
            .WithName("CreatePatient")
            .Produces<Patient>(200)
            .ProducesProblem(400)
            .WithOpenApi();
            // ./ app.MapPost("/Patient"...  

            // #####################################################
            // POST endpoint for Bundle
            // #####################################################
            app.MapPost("/Bundle", async (HttpContext httpContext) =>
            {
                // Use FhirJsonParser to parse incoming JSON as FHIR Patient
                var parser = new FhirJsonParser();
                Bundle bundle;

                try
                {
                    // Read the request body as a string
                    var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                    // Parse JSON string to FHIR Patient object
                    bundle = parser.Parse<Bundle>(requestBody);
                }
                catch (FormatException ex)
                {
                    // Return 400 Bad Request if JSON is invalid
                    return Results.BadRequest(new
                    {
                        error = "Invalid payload",
                        message = $"Failed to parse FHIR Resource: {ex.Message}"
                    });
                }

                // Check if Patient ID is present
                if (string.IsNullOrWhiteSpace(bundle.Id))
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid payload",
                        message = "Resource ID is required."
                    });
                }

                // Log details to console
                Console.WriteLine($"Received FHIR Bundle: Id={bundle.Id}");

                // Generate a new UUID for the file name
                var fileName = $"{Guid.NewGuid()}.json";
                var resourceJson = bundle.ToJson();

                if (useLocalDevFolder)
                {
                    // #####################################################
                    // Save the FHIR Resource Locally
                    // #####################################################
                    return await localFileService.SaveResourceLocally(localReceivedFolder, "Bundle", fileName, resourceJson);

                } // .if UseLocalDevFolder
                else
                {
                    // #####################################################
                    // Save the FHIR Resource to AWS S3
                    // #####################################################
                    if (s3Client == null || string.IsNullOrEmpty(s3BucketName))
                    {
                        return Results.Problem("S3 client and bucket are not configured.");
                    }

                    return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Bundle", fileName, resourceJson);
                }// .else

            })
            .WithName("CreateBundle")
            .Produces<Bundle>(200)
            .ProducesProblem(400)
            .WithOpenApi();
            // ./ app.MapPost("/Bundle"...  

            app.MapControllers();
            // #####################################################
            // Start the App
            // #####################################################
            app.Run();

        }
    }
}
