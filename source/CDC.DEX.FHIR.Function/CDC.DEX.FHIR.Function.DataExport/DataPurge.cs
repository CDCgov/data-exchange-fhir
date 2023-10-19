using System;
using CDC.DEX.FHIR.Function.DataExport;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hl7.Fhir.Rest;
using System.Net.Http.Headers;
using CDC.DEX.FHIR.Function.SharedCode;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using static Hl7.Fhir.Model.Bundle;
using System.Collections.Generic;

namespace CDC.DEX.FHIR.Function.ProcessExport
{
    public class DataPurge
    {

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly FhirEventProcessor fhirEventProcessor;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        /// <param name="configuration">App Configuration</param>
        public DataPurge(IHttpClientFactory httpClientFactory, IConfiguration configuration, FhirEventProcessor fhirEventProcessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fhirEventProcessor = fhirEventProcessor;
        }


        /// <summary>
        /// Function event trigger entry point
        /// </summary>
        /// <param name="fhirResourceToProcess">The resource created message read from the service bus queue</param>
        /// <param name="configuration">App Configuration</param>
        /// <param name="log">Function logger</param>
        [FunctionName("DataPurge")]
        public async System.Threading.Tasks.Task RunAsync([TimerTrigger("%Purge:Schedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var serializer = new FhirJsonSerializer(new SerializerSettings()
            {
                Pretty = true
            });

            string bearerToken;
            using (HttpClient authclient = httpClientFactory.CreateClient())
            {
                // get auth token
                bearerToken = await FhirServiceUtils.GetFhirServerToken(configuration, authclient);
            }
            var handler = new AuthorizationMessageHandler();
            handler.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var settings = new FhirClientSettings
            {
                PreferredFormat = ResourceFormat.Json,
            };

            var client = new FhirClient(configuration["BaseFhirUrl"], settings, handler);

            //get each profile config
            string testDestinationConfig = configuration["Export:DestinationConfig"];
            JObject testDestinationConfigJSON = JObject.Parse(testDestinationConfig);


            foreach (JObject profileConfig in testDestinationConfigJSON["Mappings"].Values<JObject>())
            {
                //take each profile and add it to a search query

                var searchParam = new SearchParams().Where($"_lastUpdated=lt{DateTime.Now.AddDays(-0).ToString("s")}");

                foreach (string profilePath in profileConfig["ProfilePathsToFilter"].Values<string>())
                {
                    searchParam.Where($"_profile={profilePath}");
                }

                Bundle results = await client.SearchAsync<Bundle>(searchParam);

                int resultCount=0;

                List<string> resourceIdsToDelete = new List<string>();
              
                do{
                    foreach (EntryComponent entryComponent in results.Entry)
                    {
                        resourceIdsToDelete.Add(entryComponent.Resource.Id);
                    }
                    resultCount += results.Entry.Count();
                }
                while ((results = await client.ContinueAsync(results)) != null);


                log.LogInformation(searchParam.ToUriParamList().ToQueryString());
                log.LogInformation(resultCount.ToString());
                //log.LogInformation(serializer.SerializeToString(results));

            }

            // delete the history and the resource




        }
    }
}
