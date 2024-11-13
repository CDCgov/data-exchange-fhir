using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FirelyApiApp.Configs;

namespace FireFacade
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
            builder.Services.Configure<FileStorageConfig>(
                builder.Configuration.GetSection(FileStorageConfig.KeyName));

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


            app.MapControllers();

            app.Run();
        }
    }
}
