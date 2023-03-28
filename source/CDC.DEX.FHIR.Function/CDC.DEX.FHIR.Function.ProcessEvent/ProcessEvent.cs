using Azure.Messaging.ServiceBus;
using CDC.DEX.FHIR.Function.SharedCode.Models;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.ProcessEvent
{
    public class ProcessEvent
    {


        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedPrepFunction</param>
        /// <param name="configuration">App Configuration</param>
        public ProcessEvent(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }


        /// <summary>
        /// Function event trigger entry point
        /// </summary>
        /// <param name="resourceCreatedMessage">The resource created message read from the service bus queue</param>
        /// <param name="configuration">App Configuration</param>
        /// <param name="log">Function logger</param>
        [FunctionName("ProcessEvent")]
        public async Task Run(
            [ServiceBusTrigger("fhireventqueue", Connection = "FhireventqueueServicebusnsfhirConnectionstring")] FhirResourceCreated resourceCreatedMessage,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            try
            {
                log.LogInformation(logPrefix() + $"Service Bus queue trigger function processed a message: {resourceCreatedMessage.ToString()}");
                //string eventMessage = eventData.EventBody.ToString();
                //FhirResourceCreated resourceCreatedMessage = JsonConvert.DeserializeObject<List<FhirResourceCreated>>(eventMessage).Single<FhirResourceCreated>();

                //AuthConfig authConfig = AuthConfig.ReadFromEnvironmentVariables();

                // GET FHIR RESOURCE SECTION 
                string requestUrl = $"{configuration["FhirUrl"]}/{resourceCreatedMessage.data.resourceType}/{resourceCreatedMessage.data.resourceFhirId}/_history/{resourceCreatedMessage.data.resourceVersionId}";

                using (HttpClient client = httpClientFactory.CreateClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    // get auth token
                    string token = await FhirServiceUtils.GetFhirServerToken(configuration, client);

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["OcpApimSubscriptionKey"]);

                    var response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    string jsonString = await response.Content.ReadAsStringAsync();

                    log.LogInformation(logPrefix() + $"FHIR Record details returned from FHIR service: {jsonString}");

                    JObject jObject = JObject.Parse(jsonString);

                    Dictionary<string, string> messagesToSend = new Dictionary<string, string>();

                    messagesToSend.Add(jObject["resourceType"].Value<string>() + " - " + jObject["id"].Value<string>(), jObject.ToString());

                    // END GET FHIR RESOURCE SECTION

                    // START WRITING TO QUEUE SECTION

                    string connectionString = configuration["FhireventqueueServicebusnsfhirConnectionstring"];
                    string queueName = configuration["FunctionPrepDownstreamQueueName"];
                    await using var serviceBusClient = new ServiceBusClient(connectionString);

                    // create the sender
                    ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

                    foreach (var keyValPair in messagesToSend)
                    {
                        // create a message that we can send. UTF-8 encoding is used when providing a string.
                        ServiceBusMessage queueMessage = new ServiceBusMessage(keyValPair.Value);

                        // send the message
                        await sender.SendMessageAsync(queueMessage);
                    }

                    // END WRITING TO QUEUE SECTION

                }

                await Task.Yield();
            }
            catch (Exception e)
            {
                // We need to keep processing the rest of the batch - capture this exception and continue.
                // Also, consider capturing details of the message that failed processing so it can be processed again later.
                exceptions.Add(e);
            }


            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }


        private string logPrefix()
        {
            return $"ProcessEvent - {DateTime.UtcNow}: ";
        }

    }
}
