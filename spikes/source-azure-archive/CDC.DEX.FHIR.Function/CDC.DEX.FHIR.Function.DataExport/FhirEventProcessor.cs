using CDC.DEX.FHIR.Function.SharedCode.Models;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.DataExport
{
    public class FhirEventProcessor
    {

        private string TruncateStrForLog(string jsonString, int maxLen)
        {
            return jsonString.Length > maxLen ? jsonString.Substring(0, maxLen) + "..." : jsonString;
        } // .TruncateStrForLog

        /// <summary>
        /// Read resource created event message and retrieve the created fhir resource
        /// </summary>
        /// <param name="resourceCreatedMessage">The resource created message read from the service bus queue</param>
        /// <param name="httpClientFactory">The configured client factory for creating http clients</param>
        /// <param name="config">App Configuration</param>
        /// <param name="log">Function logger</param>
        public async Task<JObject> ProcessFhirEvent(FhirResourceCreated resourceCreatedMessage, IHttpClientFactory httpClientFactory, IConfiguration config, ILogger log)
        {
            //EVENT SECTION

            string logPrefix = "DataExport: ";
            string resourceCreated = resourceCreatedMessage.ToString();
            log.LogInformation("{logPrefix} Service Bus queue trigger function processed a message: {resourceCreated}", logPrefix, resourceCreated);

            string requestUrl = $"{config["BaseFhirUrl"]}/{resourceCreatedMessage.data.resourceType}/{resourceCreatedMessage.data.resourceFhirId}/_history/{resourceCreatedMessage.data.resourceVersionId}";

            JObject fhirResourceToProcessJObject;

            using (HttpClient client = httpClientFactory.CreateClient())
            {

                string token = await FhirServiceUtils.GetFhirServerToken(config, client, log);
                int maxLengthForLog = 500;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header


                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    // get auth token


                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string requestUri = request.RequestUri.ToString();
                    log.LogInformation("{logPrefix}. SendAsync Start {request.RequestUri}", logPrefix, requestUri);
                    var response = await client.SendAsync(request);
                    log.LogInformation("{logPrefix}. SendAsync End", logPrefix);

                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    string jsonStringtrancate = TruncateStrForLog(jsonString, maxLengthForLog);
                    log.LogInformation("{logPrefix} FHIR Record details returned from FHIR service: {jsonStringtrancate}", logPrefix, jsonStringtrancate);
                    fhirResourceToProcessJObject = JObject.Parse(jsonString);
                    return fhirResourceToProcessJObject;
                }
            }
        }
    }
}
