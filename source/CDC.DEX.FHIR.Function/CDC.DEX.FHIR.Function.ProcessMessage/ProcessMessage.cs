
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CDC.DEX.FHIR.Function.SharedCode.Models;
using CDC.DEX.FHIR.Function.SharedCode.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.ProcessMessage
{
    public class ProcessMessage
    {

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedPrepFunction</param>
        /// <param name="configuration">App Configuration</param>
        public ProcessMessage(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        [FunctionName("ProcessMessage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            JsonNode data;
            string jsonString;

            data = JsonSerializer.Deserialize<JsonNode>(req.Body);
            jsonString = data.ToString();

            var location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle/$validate");
            PostContentBundleResult validateReportingBundleResult = await PostContentBundle(configuration, jsonString, location, log);
            JsonNode validationNode = JsonNode.Parse(validateReportingBundleResult.JsonString);
            bool isValid = validationNode["issue"][0]["diagnostics"].ToString() == "All OK";

            if (isValid)
            {
                location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle");
                JsonNode resourceNode = data["entry"][1]["resource"];
                PostContentBundleResult postResult = await PostContentBundle(configuration, resourceNode.ToJsonString(), location, log);

                data["entry"][1]["resource"] = JsonNode.Parse(postResult.JsonString);

                return new OkObjectResult(data.ToJsonString());
            }
            else
            {
                return new BadRequestObjectResult(validateReportingBundleResult.JsonString);
            }

        }

        private async Task<PostContentBundleResult> PostContentBundle(IConfiguration configuration, string bundleJson, Uri location, ILogger log)
        {
            PostContentBundleResult postContentResponse;

            log.LogInformation($"http response: {location.AbsoluteUri}");

            using (HttpClient client = httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, location) { Content = new StringContent(bundleJson, System.Text.Encoding.UTF8, "application/json") })
            {
                string token = await FhirServiceUtils.GetFhirServerToken(configuration, client);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["OcpApimSubscriptionKey"]);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                log.LogInformation($"http response: {response.IsSuccessStatusCode}");

                postContentResponse = new PostContentBundleResult() { StatusCode = response.StatusCode, JsonString = jsonString };
            }

            return postContentResponse;
        }


    }
}
