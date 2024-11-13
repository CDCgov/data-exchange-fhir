using fhirfacade.Configs;
using fhirfacade.Handlers;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace fhirfacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BundleController : ControllerBase
    {
        FileStorageConfig fileStorageConfig { get; set; }
        FhirJsonParser fhirJsonParser { get; set; }
        LocalFileService localFileService { get; set; }
        S3FileService s3FileService { get; set; }

        public BundleController(FileStorageConfig fileStorageConfig, FhirJsonParser fhirJsonParser, LocalFileService localFileService, S3FileService s3FileService)
        {
            this.fileStorageConfig = fileStorageConfig;
            this.fhirJsonParser = fhirJsonParser;
            this.localFileService = localFileService;
            this.s3FileService = s3FileService;
        }

        [HttpPost(Name = "PostBundle")]
        public BundleHandler Post([FromBody] HttpContext httpContext)
        {
            BundleHandler handler = new BundleHandler(fileStorageConfig, fhirJsonParser, localFileService, s3FileService);
            return handler;
        }
    }
}
