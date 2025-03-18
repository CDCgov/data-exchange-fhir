using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Services
{
    public interface IFileService
    {
        Task<IResult> SaveResource(string resourceType, string fileName, string content);
        Task<IResult> OpenTelemetrySaveResource(string resourceType, string fileName, string content);
    }

    public class FileServiceFactory : IFileService
    {
        private readonly LoggingUtility _loggingUtility;

        public FileServiceFactory(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility ?? throw new ArgumentNullException(nameof(loggingUtility));
        }

        public virtual IFileService CreateFileService(bool runLocally)
        {
            return runLocally
                ? new LocalFileService(_loggingUtility)
                : new S3FileService(_loggingUtility);
        }

        Task<IResult> IFileService.SaveResource(string resourceType, string fileName, string content)
        {
            return Task.FromResult<IResult>(Results.Ok(""));
        }
    }
}
