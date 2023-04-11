using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using JsonFlatten;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.ProcessExport
{
    public class ProcessExport
    {


        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        /// <param name="configuration">App Configuration</param>
        public ProcessExport(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }


        /// <summary>
        /// Function event trigger entry point
        /// </summary>
        /// <param name="fhirResourceToProcess">The resource created message read from the service bus queue</param>
        /// <param name="configuration">App Configuration</param>
        /// <param name="log">Function logger</param>
        [FunctionName("ProcessExport")]
        public async Task Run(
            [ServiceBusTrigger("fhirexportqueue", Connection = "FhirServiceBusConnectionString")] string fhirResourceToProcess,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            try
            {
                log.LogInformation(logPrefix() + $"Service Bus queue trigger function processed a message: {fhirResourceToProcess.ToString()}");

                //FeatureFlagConfig featureFlagConfig = FeatureFlagConfig.ReadFromEnvironmentVariables();
                bool flagFhirResourceCreatedExportFunctionFlatten = bool.Parse(configuration["Export:FlattenExport"]);
                bool flagFhirResourceCreatedExportFunctionUnbundle = bool.Parse(configuration["Export:UnbundleExport"]);

                using (HttpClient client = httpClientFactory.CreateClient())
                {

                    JObject jObject = JObject.Parse(fhirResourceToProcess);

                    Dictionary<string, string> filesToWrite = new Dictionary<string, string>();

                    if (jObject["resourceType"] != null && jObject["resourceType"].Value<string>() == "Bundle" && flagFhirResourceCreatedExportFunctionUnbundle)
                    {
                        // is a bundle and we will need to unbundle
                        List<JObject> unbundledFhirObjects = UnbundleFhirBundle(jObject);

                        foreach (JObject subObject in unbundledFhirObjects)
                        {
                            if (flagFhirResourceCreatedExportFunctionFlatten)
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

                        if (flagFhirResourceCreatedExportFunctionFlatten)
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

                    string accountName = configuration["Export:DatalakeStorageAccount"];

                    TokenCredential credential = new DefaultAzureCredential();

                    string blobUri = "https://" + accountName + ".blob.core.windows.net";

                    BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);

                    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration["Export:DatalakeBlobContainer"]);

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

        private string logPrefix()
        {
            return $"ProcessExport - {DateTime.UtcNow}: ";
        }

    }
}
