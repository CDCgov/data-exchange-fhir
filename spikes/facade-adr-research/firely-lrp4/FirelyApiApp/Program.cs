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
// FHIR Patient resource receive, POST endpoint at /Patient
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
        // Save the Patient Locally
        // #####################################################

        // Define the directory and file path
        var directoryPath = Path.Combine("LocalReceivedResources", "Patient");

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);

        // Serialize the patient to JSON and save it to a file asynchronously
        try
        {
            await File.WriteAllTextAsync(filePath, patientJson);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error saving patient to file: {ex.Message}");
        }

    } // .if UseLocalDevFolder
    else
    {
        // #####################################################
        // Save the Patient to AWS S3
        // #####################################################
        var putRequest = new PutObjectRequest
        {
            BucketName = s3BucketName,
            Key = $"Patient/{fileName}",
            ContentBody = patientJson
        };

        try
        {
            Console.WriteLine($"Start write for Patient: Id={patient.Id}, fileName: {fileName}");
            if (s3Client != null && s3BucketName != null)
            {
                var response = await s3Client.PutObjectAsync(putRequest); 
                Console.WriteLine($"End write for Patient: Id={patient.Id}, fileName: {fileName}, response: {response}");
            }
            else
            {
                return Results.Problem("S3 client and bucket are not configured.");
            }

        }
        catch (Exception ex)
        {
            return Results.Problem($"Error saving patient to S3: {ex.Message}");
        }

    }// .else

    // Return 201 Created response
    return Results.Created($"/Patient/{patient.Id}", patient);
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

