using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization; 
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Amazon.Runtime;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create instances of LocalFileService and S3FileService
var localFileService = new LocalFileService();
var s3FileService = new S3FileService();

// Set this via config or environment
// #####################################################
// UseLocalDevFolder to true for Local development and Not AWS
// UseLocalDevFolder to false will be using AWS
// #####################################################
var UseLocalDevFolder = builder.Configuration.GetValue<bool>("UseLocalDevFolder"); 
var UseAWSS3 = !UseLocalDevFolder;

IAmazonS3? s3Client = null; // Declare s3Client as nullable
String? s3BucketName = null;

if (UseAWSS3)
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
// Define a health check endpoint at /health
// #####################################################
app.MapGet("/health", () =>
{
    return Results.Json(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format for compatibility
        description = "API is running and healthy"
    });
}).WithOpenApi();

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
            message = $"Failed to parse FHIR Patient: {ex.Message}"
        });
    }

    // Check if Patient ID is present
    if (string.IsNullOrWhiteSpace(patient.Id))
    {
        return Results.BadRequest(new
        {
            error = "Invalid payload",
            message = "Patient ID is required."
        });
    }

    // Log patient details to console
    Console.WriteLine($"Received FHIR Patient: Id={patient.Id}");

    // Generate a new UUID for the file name
    // Not using the patient.id: var filePath = Path.Combine(directoryPath, $"{patient.Id}.json");
    var fileName = $"{Guid.NewGuid()}.json";
    var patientJson = patient.ToJson();

    if (UseLocalDevFolder)
    {
        // #####################################################
        // Save the FHIR Resource Locally
        // #####################################################
        return await localFileService.SaveResourceLocally(localReceivedFolder, "Patient", fileName, patientJson);

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

        return await s3FileService.SaveResourceToS3(s3Client, s3BucketName, "Patient", fileName, patientJson);
    }// .else

}) 
.WithName("CreatePatient")
.Produces<Patient>(201)
.ProducesProblem(400)
.WithOpenApi(); 
// ./ app.MapPost("/Patient"...  


// #####################################################
// Start the App
// #####################################################
app.Run();


