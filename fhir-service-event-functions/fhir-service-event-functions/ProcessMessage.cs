using fhir_service_event_functions.Config;
using fhir_service_event_functions.Models;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace fhir_service_event_functions
{
    public class ProcessMessage
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory">Http client factory for FhirResourceCreatedExportFunction</param>
        public ProcessMessage(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [FunctionName("ProcessMessage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            AuthConfig config = AuthConfig.ReadFromEnvironmentVariables();
            JsonNode data;
            string jsonString;

            data = JsonSerializer.Deserialize<JsonNode>(req.Body);
            jsonString = data.ToString();

            var location = new Uri($"{config.FhirUrl}/Bundle/$validate");
            PostContentBundleResult validateReportingBundleResult = await PostContentBundle(config, jsonString, location, log);
            JsonNode validationNode = JsonNode.Parse(validateReportingBundleResult.JsonString);
            bool isValid = validationNode["issue"][0]["diagnostics"].ToString() == "All OK";

            if (isValid)
            {
                location = new Uri($"{config.FhirUrl}/Bundle");
                JsonNode resourceNode = data["entry"][1]["resource"];
                PostContentBundleResult postResult = await PostContentBundle(config, resourceNode.ToJsonString(), location, log);

                return new OkObjectResult(postResult.JsonString);
            }
            else
            {
                return new BadRequestObjectResult(validateReportingBundleResult.JsonString);
            }
            
        }

        private async Task<PostContentBundleResult> PostContentBundle(AuthConfig config, string bundleJson,Uri location ,ILogger log)
        {
            PostContentBundleResult postContentResponse;

            log.LogInformation($"http response: {location.AbsoluteUri}");
            
            using (HttpClient client = httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, location) { Content = new StringContent(bundleJson, System.Text.Encoding.UTF8, "application/json") })
            {
                string token = await GetFhirServerToken(config, client);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Ocp-Apim-Subscription-Key", config.OcpApimSubscriptionKey);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                log.LogInformation($"http response: {response.IsSuccessStatusCode}");

                postContentResponse = new PostContentBundleResult() { StatusCode = response.StatusCode, JsonString = jsonString };
            }

            return postContentResponse;
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

    }
}
