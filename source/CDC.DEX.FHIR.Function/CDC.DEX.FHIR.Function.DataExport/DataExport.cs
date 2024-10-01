using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using CDC.DEX.FHIR.Function.SharedCode.Models;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using JsonFlatten;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.DataExport
{
    public class DataExport
    {


        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly FhirEventProcessor fhirEventProcessor;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        /// <param name="configuration">App Configuration</param>
        public DataExport(IHttpClientFactory httpClientFactory, IConfiguration configuration,FhirEventProcessor fhirEventProcessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fhirEventProcessor =  fhirEventProcessor;
        }


        /// <summary>
        /// Function event trigger entry point
        /// </summary>
        /// <param name="fhirResourceToProcess">The resource created message read from the service bus queue</param>
        /// <param name="configuration">App Configuration</param>
        /// <param name="log">Function logger</param>
        [FunctionName("DataExport")]
        public async Task Run(
            [ServiceBusTrigger("fhireventqueue", Connection = "FhirServiceBusConnectionString")] FhirResourceCreated resourceCreatedMessage,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            try
            {
                //EVENT SECTION
                string logPrefix = LogPrefix();

                log.LogInformation("{logPrefix} ProcessFhirEvent Start", logPrefix);

                JObject fhirResourceToProcessJObject = await fhirEventProcessor.ProcessFhirEvent(resourceCreatedMessage, httpClientFactory, configuration, log);

                log.LogInformation("{logPrefix} ProcessFhirEvent Done", logPrefix);

                //EXPORT SECTION

                //log.LogInformation(logPrefix() + $"Service Bus queue trigger function processed a message: {fhirResourceToProcess.ToString()}");

                bool flagFhirResourceCreatedExportFunctionFlatten = bool.Parse(configuration["Export:FlattenExport"]);
                bool flagFhirResourceCreatedExportFunctionUnbundle = bool.Parse(configuration["Export:UnbundleExport"]);

                Dictionary<string, string> filesToWrite = new Dictionary<string, string>();


                // DESTINATION DETERMINATION

                // default SA and container path
                string storageAccountName = configuration["Export:DatalakeStorageAccount"];
                string blobContainerPath = configuration["Export:DatalakeBlobContainer"];
                string SATenantIdConfigName = "Export:EDAVTenatId";
                string SAClientIdConfigName = "Export:EDAVClientId";
                string SAClientSecretConfigName = "Export:EDAVClientSecret";


                string testDestinationConfig = configuration["Export:DestinationConfig"];
                JObject testDestinationConfigJSON = JObject.Parse(testDestinationConfig);

                if (fhirResourceToProcessJObject.ContainsKey("meta") && fhirResourceToProcessJObject["meta"].Value<JObject>().ContainsKey("profile"))
                {
                    // potentially change how the resources are sorted
                    string validationProfile = fhirResourceToProcessJObject["meta"]["profile"][0].Value<string>();

                    foreach (JObject profileConfig in testDestinationConfigJSON["Mappings"].Values<JObject>())
                    {
                        foreach (string profilePath in profileConfig["ProfilePathsToFilter"].Values<string>())
                        {
                            if (validationProfile.Trim() == profilePath.Trim())
                            {
                                storageAccountName = configuration[profileConfig["StorageAccount"].Value<string>().Trim()];
                                blobContainerPath = configuration[profileConfig["BlobContainerPath"].Value<string>().Trim()];
                                SATenantIdConfigName = profileConfig["SAServicePrincipalTenantIdConfigName"].Value<string>().Trim();
                                SAClientIdConfigName = profileConfig["SAServicePrincipalClientIdConfigName"].Value<string>().Trim();
                                SAClientSecretConfigName = profileConfig["SAServicePrincipalClientSecretConfigName"].Value<string>().Trim();
                            }
                        }

                    }

                }

                // get auth for SA
                log.LogInformation("{logPrefix} ClientSecretCredential Start", logPrefix);
                string enantIdConfig = configuration[SATenantIdConfigName]
                log.LogInformation("{logPrefix} ClientSecretCredential SATenantIdConfigName {config}", logPrefix, configenantId);
                string clientIdConfig = configuration[SATenantIdConfigName]
                log.LogInformation(" {logPrefix} ClientSecretCredential SAClientIdConfigName {clientIdConfig}" , logPrefix, clientIdConfig);


                TokenCredential credential = new ClientSecretCredential(
                    configuration[SATenantIdConfigName],
                    configuration[SAClientIdConfigName],
                    configuration[SAClientSecretConfigName]);
                log.LogInformation("{logPrefix} ClientSecretCredential End", logPrefix);

                // DATA EXPORT PROCESSING

                if (fhirResourceToProcessJObject["resourceType"] != null && fhirResourceToProcessJObject["resourceType"].Value<string>() == "Bundle" && flagFhirResourceCreatedExportFunctionUnbundle)
                {
                    // is a bundle and unbundle flag is true, need to unbundle
                    List<JObject> unbundledFhirObjects = UnbundleFhirBundle(fhirResourceToProcessJObject);

                    foreach (JObject subObject in unbundledFhirObjects)
                    {
                        if (flagFhirResourceCreatedExportFunctionFlatten)
                        {
                            //flatten
                            string flattenedJson = FlattenJsonResource(subObject);

                            string pathToWrite = subObject["resourceType"].Value<string>();
                            //get profile data for sorting bundles
                            pathToWrite += "/" + fhirResourceToProcessJObject["id"].Value<string>();
                            pathToWrite += "_" + subObject["id"].Value<string>();
                            filesToWrite.Add(pathToWrite, flattenedJson.ToString());
                        }
                        else
                        {
                            //no flatten
                            string pathToWrite = subObject["resourceType"].Value<string>();
                            //get profile data for sorting bundles
                            pathToWrite += "/" + subObject["id"].Value<string>();
                            filesToWrite.Add(pathToWrite, subObject.ToString());
                        }
                    }
                }
                else
                {
                    // a single entry no need to unbundle

                    if (flagFhirResourceCreatedExportFunctionFlatten)
                    {
                        //flatten
                        string flattenedJson = FlattenJsonResource(fhirResourceToProcessJObject);

                        string pathToWrite = fhirResourceToProcessJObject["resourceType"].Value<string>();
                        //get profile data for sorting bundles

                        pathToWrite += "/" + configuration["Platfom"] + "/Source/" + fhirResourceToProcessJObject["id"].Value<string>();
                        filesToWrite.Add(pathToWrite, flattenedJson.ToString());
                    }
                    else
                    {
                        string pathToWrite = fhirResourceToProcessJObject["resourceType"].Value<string>();
                        //get profile data for sorting bundles

                        pathToWrite += "/" + configuration["Platform"] + "/Source/" + fhirResourceToProcessJObject["id"].Value<string>();


                        filesToWrite.Add(pathToWrite, fhirResourceToProcessJObject.ToString());
                    }
                }




                // END GET FHIR RESOURCE SECTION

                // START WRITING TO DATA LAKE SECTION


                log.LogInformation("{logPrefix} Upload Start", logPrefix);
                string blobUri = $"https://{storageAccountName}.blob.core.windows.net";

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);

                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerPath);

                foreach (var keyValPair in filesToWrite)
                {
                    string keyValueKey = keyValPair.Key;
                    BlobClient blobClient = blobContainerClient.GetBlobClient(keyValueKey);
                    log.LogInformation("{logPrefix} Writing data to file {keyValueKey}", logPrefix, keyValueKey);
                    blobClient.Upload(BinaryData.FromString(keyValPair.Value), true);
                }
                log.LogInformation("{logPrefix} Upload End", logPrefix);
                // END WRITING TO DATA LAKE SECTION


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

        /// <summary>
        /// Flatten all entries within this json object, then reconstruct the now-flattened json
        /// </summary>
        /// <param name="bundleJObject">The top level JOBject to be flattened</param>
        public string FlattenJsonResource(JObject jsonToFlatten)
        {
            Dictionary<string, object> flattenedObject = new Dictionary<string, object>(jsonToFlatten.Flatten());

            return JsonConvert.SerializeObject(flattenedObject);

        }

        public static string LogPrefix()
        {
            return $"DataExport - {DateTime.UtcNow}: ";
        }

    }
}
