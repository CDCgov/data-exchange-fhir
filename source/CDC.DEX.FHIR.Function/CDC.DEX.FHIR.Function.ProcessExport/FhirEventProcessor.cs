using CDC.DEX.FHIR.Function.SharedCode.Models;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.DataExport
{
    public class FhirEventProcessor
    {

        /// <summary>
        /// Read resource created event message and retrieve the created fhir resource
        /// </summary>
        /// <param name="resourceCreatedMessage">The resource created message read from the service bus queue</param>
        /// <param name="httpClientFactory">The configured client factory for creating http clients</param>
        /// <param name="config">App Configuration</param>
        /// <param name="log">Function logger</param>
        public async Task<JObject>  ProcessFhirEvent(FhirResourceCreated resourceCreatedMessage, IHttpClientFactory httpClientFactory, IConfiguration config, ILogger log)
        {
            //EVENT SECTION

            log.LogInformation(DataExport.LogPrefix() + $"Service Bus queue trigger function processed a message: {resourceCreatedMessage.ToString()}");

            string requestUrl = $"{config["BaseFhirUrl"]}/{resourceCreatedMessage.data.resourceType}/{resourceCreatedMessage.data.resourceFhirId}/_history/{resourceCreatedMessage.data.resourceVersionId}";

            JObject fhirResourceToProcessJObject;

            using (HttpClient client = httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                // get auth token
                string token = await FhirServiceUtils.GetFhirServerToken(config, client);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Ocp-Apim-Subscription-Key", config["OcpApimSubscriptionKey"]);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                log.LogInformation(DataExport.LogPrefix() + $"FHIR Record details returned from FHIR service: {jsonString}");

                fhirResourceToProcessJObject = JObject.Parse(jsonString);

                return fhirResourceToProcessJObject;
            }
        }
    }
}
