using OneCDPFHIRFacade.Utilities;
using System;
using System.Threading.Tasks;

namespace OneCDPFHIRFacade.Services
{
    public interface IFileService
    {
        Task<IResult> SaveResource(string resourceType, string fileName, string content);
    }

    public class FileServiceFactory
    {
        private readonly LoggingUtility _loggingUtility;

        public FileServiceFactory(LoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility ?? throw new ArgumentNullException(nameof(loggingUtility));
        }

        public IFileService CreateFileService(bool runLocally)
        {
            return runLocally
                ? new LocalFileService(_loggingUtility)
                : new S3FileService(_loggingUtility);
        }
    }
}
