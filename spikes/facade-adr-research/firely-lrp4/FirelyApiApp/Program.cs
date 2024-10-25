using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

    // #####################################################
    // Save the Patient Resource to a local folder, TODO change to AWS S3
    // #####################################################
        // Define the directory and file path
    var directoryPath = Path.Combine("LocalReceivedResources", "Patient");
    var filePath = Path.Combine(directoryPath, $"{patient.Id}.json");

    // Ensure the directory exists
    Directory.CreateDirectory(directoryPath);

    // Serialize the patient to JSON and save it to a file asynchronously
    try
    {
        await File.WriteAllTextAsync(filePath, patient.ToJson());
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error saving patient to file: {ex.Message}");
    }

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

