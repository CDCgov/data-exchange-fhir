using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using fhir_service_event_functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Core;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using JsonFlatten;
using fhir_service_event_functions.Config;

namespace fhir_service_event_functions
{
    public class FhirResourceCreatedExportFunction
    {


        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        public FhirResourceCreatedExportFunction(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }


        /// <summary>
        /// Function event trigger entry point
        /// </summary>
        /// <param name="events">The events triggered from the listened to event hub</param>
        /// <param name="log">Function logger</param>
        [FunctionName("FhirResourceCreatedExportFunction")]
        public async Task Run(
            [EventHubTrigger("eventhubfhir",Connection = "eventhubnsfhir_RootManageSharedAccessKey_EVENTHUB")] EventData[] events,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    log.LogInformation(logPrefix()+ $"Event Hub trigger function processed a message: {eventData.EventBody}");
                    string eventMessage = eventData.EventBody.ToString();
                    EventHubFhirResourceCreated eventHubMessage = JsonConvert.DeserializeObject<List<EventHubFhirResourceCreated>>(eventMessage).Single<EventHubFhirResourceCreated>();

                    AuthConfig authConfig = AuthConfig.ReadFromEnvironmentVariables();
                    FeatureFlagConfig featureFlagConfig = FeatureFlagConfig.ReadFromEnvironmentVariables();

                    // GET FHIR RESOURCE SECTION 

                    string requestUrl = $"{authConfig.FhirUrl}/{eventHubMessage.data.resourceType}/{eventHubMessage.data.resourceFhirId}/_history/{eventHubMessage.data.resourceVersionId}";

                    using (HttpClient client = httpClientFactory.CreateClient())
                    using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                    {
                        // get auth token
                        string token = await GetFhirServerToken(authConfig, client);

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        request.Headers.Add("Ocp-Apim-Subscription-Key", authConfig.OcpApimSubscriptionKey);

                        var response = await client.SendAsync(request);

                        response.EnsureSuccessStatusCode();

                        string jsonString = await response.Content.ReadAsStringAsync();

                        log.LogInformation(logPrefix() + $"FHIR Record details returned from FHIR service: {jsonString}");

                        JObject jObject = JObject.Parse(jsonString);

                        Dictionary<string, string> filesToWrite = new Dictionary<string, string>();

                        if (jObject["resourceType"] != null && jObject["resourceType"].Value<string>() == "Bundle" && featureFlagConfig.FhirResourceCreatedExportFunctionUnbundle)
                        {
                            // is a bundle and we will need to unbundle
                            List<JObject> unbundledFhirObjects = UnbundleFhirBundle(jObject);

                            foreach (JObject subObject in unbundledFhirObjects)
                            {
                                if (featureFlagConfig.FhirResourceCreatedExportFunctionFlatten)
                                {
                                    //flatten
                                    Dictionary<string, object> flattenedObject = new Dictionary<string, object>(subObject.Flatten());

                                    StringBuilder contentToWriteToFile = new StringBuilder();
                                    foreach (var keyValPair in flattenedObject)
                                    {
                                        contentToWriteToFile.Append($"\"{keyValPair.Key}\":\"{keyValPair.Value}\" \n");
                                    }

                                    filesToWrite.Add(subObject["resourceType"].Value<string>() + " - " + jObject["id"] + "_" + subObject["id"].Value<string>(), contentToWriteToFile.ToString());
                                }
                                else
                                {
                                    //no flatten
                                    filesToWrite.Add(subObject["resourceType"].Value<string>() + " - " + jObject["id"] + "_" + subObject["id"].Value<string>(), subObject.ToString());

                                }
                            }
                        }
                        else
                        {
                            // a single entry no need to unbundle

                            if (featureFlagConfig.FhirResourceCreatedExportFunctionFlatten)
                            {
                                //flatten
                                Dictionary<string, object> flattenedObject = new Dictionary<string, object>(jObject.Flatten());

                                StringBuilder contentToWriteToFile = new StringBuilder();
                                foreach (var keyValPair in flattenedObject)
                                {
                                    contentToWriteToFile.Append($"\"{keyValPair.Key}\":\"{keyValPair.Value}\" \n");
                                }

                                filesToWrite.Add(jObject["resourceType"].Value<string>() + " - " + jObject["id"].Value<string>(), contentToWriteToFile.ToString());
                            }
                            else
                            {
                                filesToWrite.Add(jObject["resourceType"].Value<string>() + " - " + jObject["id"].Value<string>(), jObject.ToString());
                            }
                        }

                        // END GET FHIR RESOURCE SECTION

                        // START WRITING TO DATA LAKE SECTION

                        string accountName = Environment.GetEnvironmentVariable("DatalakeStorageAccountName");

                        TokenCredential credential = new DefaultAzureCredential();

                        string blobUri = "https://" + accountName + ".blob.core.windows.net";

                        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);

                        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("DatalakeBlobContainerName"));

                        foreach (var keyValPair in filesToWrite)
                        {
                            BlobClient blobClient = blobContainerClient.GetBlobClient($"{keyValPair.Key}");
                            log.LogInformation(logPrefix() + $"Writing data to file {keyValPair.Key}: \n {keyValPair.Value}");
                            await blobClient.UploadAsync(BinaryData.FromString($"{keyValPair.Value}"), true);
                        }

                        // END WRITING TO DATA LAKE SECTION

                    }



                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        /// <summary>
        /// Get the service principle to access to the azure fhir server
        /// </summary>
        /// <param name="authConfig">The environment specfic authConfig</param>
        /// <param name="httpClient">The httpClient to use to get the fhir token</param>
        private async Task<string> GetFhirServerToken(AuthConfig authConfig, HttpClient httpClient)
        {
            string token;

            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "Client_Credentials");
            dict.Add("client_id", authConfig.ClientId);
            dict.Add("client_secret", authConfig.ClientSecret);

            using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{authConfig.FhirUrl}/auth") { Content = new FormUrlEncodedContent(dict) })
            {
                tokenRequest.Headers.Add("Ocp-Apim-Subscription-Key", authConfig.OcpApimSubscriptionKey);

                var tokenResponse = await httpClient.SendAsync(tokenRequest);

                tokenResponse.EnsureSuccessStatusCode();

                var result = await tokenResponse.Content.ReadFromJsonAsync<AuthTokenResult>();

                //log.LogInformation(logPrefix()+ "Token acquired \n");

                token = result!.access_token;
            }

            return token;
        }

        /// <summary>
        /// Unbundle any resources within the entry property of the bundle, recursively unbundling any sub-bundles found
        /// </summary>
        /// <param name="bundleJObject">The top level JOBject of the bundle to be unbundled</param>
        public List<JObject> UnbundleFhirBundle(JObject bundleJObject)
        {
            List<JObject> unbundledObjects = new List<JObject>();

            if (bundleJObject == null || !bundleJObject.HasValues) return unbundledObjects;

            if (bundleJObject["entry"] != null)
            {
                foreach (JObject entryResource in bundleJObject["entry"])
                {
                    JObject entry = entryResource["resource"].Value<JObject>();
                    if (entry["resourceType"] != null && entry["resourceType"].Value<string>() == "Bundle")
                    {
                        unbundledObjects.AddRange(UnbundleFhirBundle(entry));
                    }
                    else
                    {
                        unbundledObjects.Add(entry);
                    }
                }
            }

            return unbundledObjects;
        }

        private string logPrefix()
        {
            return $"FhirResourceCreatedExportFunction - {DateTime.UtcNow}: ";
        }

    }
}
