using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace fhir_service_event_functions
{
    public class ProcessMessage
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        public ProcessMessage(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [FunctionName("ProcessMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string json = data.ToString();

            var parser = new FhirJsonParser();

            var reportingBundle = parser.Parse<Bundle>(json);
            var bundle = parser.Parse<Bundle>(reportingBundle.Entry[1].Resource.ToJson());

            string responseMessage = bundle.ToJson();

            return new OkObjectResult(responseMessage);
        }
    }
}
