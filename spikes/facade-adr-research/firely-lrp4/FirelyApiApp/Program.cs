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

// ##########################################
// Define a health check endpoint at /health
// ##########################################
app.MapGet("/health", () =>
{
    return Results.Json(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format for compatibility
        description = "API is running and healthy"
    });
}).WithOpenApi();

// ##########################################
// FHIR Patient resource receive, POST endpoint at /Patient
// ##########################################
app.MapPost("/Patient", (Patient patient) =>
{
    // Check if the Patient ID is null or empty
    if (string.IsNullOrWhiteSpace(patient.Id))
    {
        // Return 400 Bad Request with an error message
        return Results.BadRequest(new
        {
            error = "Invalid payload",
            message = "Patient ID is required."
        });
    }

    // Log the patient information to the console
    Console.WriteLine($"Received Patient: Id={patient.Id}, Name={patient.Name}, Age={patient.Age}");

    // Return 201 Created response
    return Results.Created($"/Patient/{patient.Id}", patient);
}).WithName("CreatePatient")  // Optional: Name the endpoint
.Produces<Patient>(201)     // Document 201 response
.ProducesProblem(400)       // Document 400 Bad Request response
.WithOpenApi();   

// ##########################################
// Start the App
// ##########################################
app.Run();


    public class Patient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }