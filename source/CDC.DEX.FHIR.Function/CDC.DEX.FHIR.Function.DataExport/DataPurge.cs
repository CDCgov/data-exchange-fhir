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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        /// <param name="configuration">App Configuration</param>
        public DataPurge(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }


        /// <summary>
        /// Function timed trigger entry point
        /// </summary>
        /// <param name="triggerSchedule">The timer schedule which the function is triggered off of, configured dynamically through config</param>
        /// <param name="log">Function logger</param>
        [FunctionName("DataPurge")]
        public async System.Threading.Tasks.Task RunAsync([TimerTrigger("%Purge:Schedule%")] TimerInfo triggerSchedule, ILogger log)
        {
            string dateTime = DateTime.Now;
            log.LogInformation"DataPurge Timer trigger function executed at: {dateTime}", dateTime);

            var serializer = new FhirJsonSerializer(new SerializerSettings()
            {
                Pretty = true
            });

            string bearerToken;
            using (HttpClient authclient = httpClientFactory.CreateClient())
            {
                // get auth token
                bearerToken = await FhirServiceUtils.GetFhirServerToken(configuration, authclient,log);
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

            Dictionary<string,string> resourceIdsToDelete = new Dictionary<string,string>();

            foreach (JObject profileConfig in testDestinationConfigJSON["Mappings"].Values<JObject>())
            {
                //take each profile and add it to a search query,
                //but only if it has a HoursToRetain (if it's missing or less than 1, never purge)

                if (profileConfig.ContainsKey("HoursToRetain") && profileConfig["HoursToRetain"].Value<int>() >= 0)
                {

                    int hoursToRetain = profileConfig["HoursToRetain"].Value<int>();

                    var searchParam = new SearchParams().Where($"_lastUpdated=le{DateTime.UtcNow.AddHours(-hoursToRetain).ToString("s")}");

                    foreach (string profilePath in profileConfig["ProfilePathsToFilter"].Values<string>())
                    {
                        searchParam.Where($"_profile={profilePath}");
                    }

                    Bundle results = await client.SearchAsync<Bundle>(searchParam);

                    do
                    {
                        foreach (EntryComponent entryComponent in results.Entry)
                        {
                            resourceIdsToDelete.Add(entryComponent.Resource.Id, entryComponent.Resource.TypeName);
                        }
                    }
                    while ((results = await client.ContinueAsync(results)) != null);

                }
            }

            // delete the history and the resource
            foreach(string resourceId in resourceIdsToDelete.Keys)
            {
                SearchParams searchParams = new SearchParams();
                searchParams.Add("_id", resourceId);
                searchParams.Add("hardDelete", "true");
                await client.DeleteAsync(resourceIdsToDelete[resourceId], searchParams);
                string resourceIdsDelete = resourceIdsToDelete[resourceId]/resourceId;
                log.LogInformation("Hard Delete Successful: {resourceIdDeletes}", resourceIdsDelete);
            }


        }
    }
}
