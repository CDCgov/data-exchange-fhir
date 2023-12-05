
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
using System.Text.RegularExpressions;

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

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                JsonNode data;
                string jsonString;

                bool flagProcessMessageFunctionSkipValidate = bool.Parse(configuration["FunctionProcessMessage:SkipValidation"]);

                data = JsonSerializer.Deserialize<JsonNode>(req.Body);
                jsonString = data.ToString();

                log.LogInformation("ProcessMessage bundle received: " + jsonString);

                var location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle/$validate");

                string cleanedBearerToken = CleanBearerToken(req.Headers["Authorization"]);

                PostContentBundleResult validateReportingBundleResult = await PostContentBundle(configuration, jsonString, location, cleanedBearerToken, log);

                log.LogInformation("ProcessMessage validation done with result: " + validateReportingBundleResult.JsonString);

                bool isValid;
                if (flagProcessMessageFunctionSkipValidate)
                {
                    log.LogInformation("Skipping ProcessMessage Validation");
                    isValid = true;
                }
                else
                {
                    isValid = !validateReportingBundleResult.JsonString.Contains("\"severity\":\"error\"");
                }

                ContentResult contentResult = new ContentResult();
                contentResult.ContentType = "application/fhir+json";

                if (isValid)
                {
                    location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle");

                    // Submit the entire message bundle instead of just the content bundle
                    //JsonNode resourceNode = data["entry"][1]["resource"];
                    JsonNode messageNode = data;

                    PostContentBundleResult postResult = await PostContentBundle(configuration, messageNode.ToJsonString(), location, cleanedBearerToken, log);

                    //data["entry"][1]["resource"] = JsonNode.Parse(postResult.JsonString);
                    data = JsonNode.Parse(postResult.JsonString);

                    contentResult.Content = data.ToJsonString();
                    contentResult.StatusCode = 201;
                    return contentResult;
                }
                else
                {
                    contentResult.Content = validateReportingBundleResult.JsonString;
                    contentResult.StatusCode = 422;
                    return contentResult;
                }
            }
            catch (HttpRequestException e)
            {
                ContentResult contentResult = new ContentResult();
                contentResult.ContentType = "application/fhir+json";
                contentResult.StatusCode = ((int)e.StatusCode.Value);
                return contentResult;
            }

        }

        private async Task<PostContentBundleResult> PostContentBundle(IConfiguration configuration, string bundleJson, Uri location, string bearerToken, ILogger log)
        {
            PostContentBundleResult postContentResponse;

            log.LogInformation($"http response: {location.AbsoluteUri}");

            using (HttpClient client = httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, location) { Content = new StringContent(bundleJson, System.Text.Encoding.UTF8, "application/json") })
            {

                //passthrough the cleaned auth bearer token used
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                // add the Ocp-Apim-Subscription-Key if it's configured
                request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["OcpApimSubscriptionKey"]);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                log.LogInformation($"http response: {response.IsSuccessStatusCode}");

                postContentResponse = new PostContentBundleResult() { StatusCode = response.StatusCode, JsonString = jsonString };
            }

            return postContentResponse;
        }

        private string CleanBearerToken(string bearerToken)
        {
            Regex regexString = new Regex("[^a-zA-Z0-9\\.\\-_ ]");
            string cleanedBearerToken = regexString.Replace(bearerToken, "");
            cleanedBearerToken = cleanedBearerToken.Replace("Bearer ", "");

            return cleanedBearerToken;
        }


    }
}
