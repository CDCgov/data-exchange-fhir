// LocalFileService.cs
// #####################################################
// SaveResourceLocally
// #####################################################

using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Services
{
    public interface ILocalFileService
    {
        Task<IResult> SaveResourceLocally(string folderPath, string resourceType, string fileName, string content);
    }
    public class LocalFileService : ILocalFileService
    {
        private readonly LoggingUtility _loggingUtility;
        public LocalFileService(LoggingUtility loggingUtility)
        {
            this._loggingUtility = loggingUtility;
        }
        public async Task<IResult> SaveResourceLocally(string baseDirectory, string subDirectory, string fileName, string resourceJson)
        {
            // Define the directory and file path
            var directoryPath = Path.Combine(baseDirectory, subDirectory);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Define the full path for the file
            var filePath = Path.Combine(directoryPath, fileName);

            // Serialize the resource to JSON and save it to a file asynchronously
            try
            {
                await File.WriteAllTextAsync(filePath, resourceJson);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error saving resource to file: {ex.Message}");
            }

            await _loggingUtility.Logging($"Resource saved successfully at {filePath}");
            return Results.Ok($"Resource saved successfully at {filePath}");
        }// .SaveResourceLocally
    }// .LocalFileService

}// .namespace

