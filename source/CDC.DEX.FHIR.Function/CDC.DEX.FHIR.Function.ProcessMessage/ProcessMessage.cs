
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CDC.DEX.FHIR.Function.SharedCode.Models;
// using CDC.DEX.FHIR.Function.SharedCode.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            DateTime startProcessMessage = DateTime.Now;
            ContentResult contentResult = new ContentResult
            {
                ContentType = "application/fhir+json"
            };

            try
            {
                log.LogInformation("ProcessMessage HTTP trigger function received a request.");

                // guard for missing Authorization in Headers
                const string authorizationKeyName = "Authorization";
                if (!req.Headers.ContainsKey(authorizationKeyName)) 
                {
                    const string errorMessage =  $"Headers missing {authorizationKeyName}";
                    log.LogError(errorMessage);
                    contentResult.Content = JsonErrorStr(errorMessage);
                    contentResult.StatusCode = 401;
                    return contentResult;
                } // .if

                bool flagProcessMessageFunctionSkipValidate = bool.Parse(configuration["FunctionProcessMessage:SkipValidation"]);
                
                // guard for empty request body, no payload
                if (req.ContentLength == 0) {
                    const string errorMessage =  $"request body content length null";
                    log.LogError(errorMessage);
                    contentResult.Content = JsonErrorStr(errorMessage);
                    contentResult.StatusCode = 400;
                    return contentResult;
                } // .if

                // try to deserialize body payload to json
                JsonNode data;
                string jsonString = string.Empty;
                try 
                {
                    data = JsonSerializer.Deserialize<JsonNode>(req.Body);
                    jsonString = data.ToString() ?? string.Empty;
                } // .try
                catch (JsonException  e) 
                {
                  
                    log.LogError(e.ToString());
                    contentResult.Content = JsonErrorStr("error deserialize received JSON");
                    contentResult.StatusCode = 400;
                    return contentResult;
                } // .catch

                log.LogInformation("ProcessMessage bundle received");

                var location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle/$validate");

                string cleanedBearerToken = CleanBearerToken(req.Headers[authorizationKeyName]);

                DateTime startFHIRValidation = DateTime.Now;
                PostContentBundleResult validateReportingBundleResult = await PostContentBundle(configuration, jsonString, location, cleanedBearerToken, log);
                TimeSpan durationFHIRValidation = DateTime.Now - startFHIRValidation;
                string statusCode = validateReportingBundleResult.StatusCode;
                log.LogInformation("ProcessMessage FHIR validation done with result: {statusCode}", statusCode);
                log.LogInformation($"ProcessMessage FHIR validation run duration ms: {durationFHIRValidation.Milliseconds}");
                
                // log.LogInformation("ProcessMessage validation done with result: " + validateReportingBundleResult.JsonString);

                bool isValid;
                if (flagProcessMessageFunctionSkipValidate)
                {
                    log.LogInformation("ProcessMessage Skipping FHIR Validation");
                    isValid = true;
                }
                else
                {
                    isValid = !validateReportingBundleResult.JsonString.Contains("\"severity\":\"error\"");
                }

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
            // catch (HttpRequestException e)
            catch( Exception e)
            {
                if (e is HttpRequestException httpException) // exception returned from the FHIR server call
                {
                    contentResult.Content = JsonErrorStr($"http error {httpException.StatusCode}");
                    contentResult.StatusCode = (int)httpException.StatusCode;
                }
                else // something else (exception) happened
                {
                    contentResult.Content = JsonErrorStr("unexpected condition was encountered");
                    contentResult.StatusCode = 500; // code for internal server error as exception
                }
                log.LogError(e.ToString());

                return contentResult;

            } // .catch
            finally 
            {
                TimeSpan durationProcessMessage = DateTime.Now - startProcessMessage;
                log.LogInformation($"ProcessMessage total run duration ms: {durationProcessMessage.Milliseconds}");

            }

        } // .run

        private async Task<PostContentBundleResult> PostContentBundle(IConfiguration configuration, string bundleJson, Uri location, string bearerToken, ILogger log)
        {
            PostContentBundleResult postContentResponse;

            log.LogInformation($"ProcessMessage, PostContentBundle sending for validation to FHIR server endpoint: {location.AbsoluteUri}");

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

                log.LogInformation($"ProcessMessage, PostContentBundle received from FHIR server http response status code success: {response.IsSuccessStatusCode}");

                postContentResponse = new PostContentBundleResult() { StatusCode = response.StatusCode, JsonString = jsonString };
            }

            return postContentResponse;
        }

        private static string CleanBearerToken(string bearerToken)
        {
            Regex regexString = new Regex("[^a-zA-Z0-9\\.\\-_ ]");
            string cleanedBearerToken = regexString.Replace(bearerToken, "");
            cleanedBearerToken = cleanedBearerToken.Replace("Bearer ", "");

            return cleanedBearerToken;
        }

        private static string JsonErrorStr(string errorMessage)
        {
            return (new JsonObject
            {
                ["error"] = errorMessage.ToLower(),
            }).ToJsonString();
        } // .JsonErrorStr

        private static string TruncateStrForLog(string jsonString, int maxLen)   
        {
            return jsonString.Length > maxLen ? jsonString.Substring(0, maxLen) + "..." : jsonString;
        } // .TruncateStrForLog

        [FunctionName("Health")]
        public async Task<IActionResult> RunHealthCheck(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            ContentResult contentResult = new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int) HttpStatusCode.OK  // 200
            };

            return contentResult;
        }

    } // .class 
} // .namespace 
