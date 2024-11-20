using Hl7.Fhir.Serialization;
using OneCDPFHIRFacade.Configs;

namespace OneCDPFHIRFacade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            // Set this via config or environment
            // #####################################################
            // UseLocalDevFolder to true for Local development and Not AWS
            // UseLocalDevFolder to false will be using AWS
            // #####################################################
            FileStorageConfig fileStorageConfig = new FileStorageConfig();
            builder.Configuration.GetSection(FileStorageConfig.KeyName).Bind(fileStorageConfig);
            builder.Services.AddSingleton(fileStorageConfig);

            AWSConfig awsConfig = new AWSConfig();
            builder.Configuration.GetSection(AWSConfig.KeyName).Bind(awsConfig);
            builder.Services.AddSingleton(awsConfig);

            builder.Services.AddSingleton<LocalFileService>();
            builder.Services.AddSingleton<S3FileService>();
            builder.Services.AddSingleton<FhirJsonParser>();

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Optional: Customize Newtonsoft.Json settings here
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
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

            app.Run();
        }
    }
}
