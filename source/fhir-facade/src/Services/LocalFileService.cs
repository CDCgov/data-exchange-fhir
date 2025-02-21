using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OneCDPFHIRFacade.Services
{
 
    public class LocalFileService : IFileService
 
    {
        private readonly LoggingUtility _loggingUtility;

        public LocalFileService(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility ?? throw new ArgumentNullException(nameof(loggingUtility));
        }

        public async Task<IResult> SaveResource(string resourceType, string fileName, string content)
        {
            // Define the directory and file path
            var directoryPath = Path.Combine(LocalFileStorageConfig.LocalDevFolder!,resourceType);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Define the full path for the file
            var filePath = Path.Combine(directoryPath, fileName);

            // Serialize the resource to JSON and save it to a file asynchronously
            try
            {
                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving resource to file: {ex.Message}");
            }

            // Log successful save and return the result
            await _loggingUtility.Logging($"Resource saved successfully at {filePath}");
            return Results.Ok($"Resource saved successfully at {filePath}");
        }
    }
}
