
using CDC.DEX.FHIR.Function.SharedCode.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string prefix = "ProcessMessage: ";
            DateTime startProcessMessage = DateTime.Now;
            ContentResult contentResult = new ContentResult
            {
                ContentType = "application/fhir+json"
            };

            try
            {
                log.LogInformation("{prefix} ProcessMessage HTTP trigger function received a request.", prefix);

                // limit log of bundles or validation result to the first 300 chars
                const int maxLengthForLog = 300;

                // guard for missing Authorization in Headers
                const string authorizationKeyName = "Authorization";
                if (!req.Headers.ContainsKey(authorizationKeyName))
                {
                    const string errorMessage = $"Headers missing {authorizationKeyName}";
                    log.LogError(errorMessage);
                    contentResult.Content = JsonErrorStr(errorMessage);
                    contentResult.StatusCode = StatusCodes.Status401Unauthorized;
                    return contentResult;
                } // .if

       

                bool flagProcessMessageFunctionSkipValidate = bool.Parse(configuration["FunctionProcessMessage:SkipValidation"]);
               // guard for empty request body, no payload
                if (req.ContentLength == 0)
                {
                    const string errorMessage = $"request body content length null";
                    log.LogError(errorMessage);
                    contentResult.Content = JsonErrorStr(errorMessage);
                    contentResult.StatusCode = StatusCodes.Status400BadRequest;
                    return contentResult;
                } 

                // try to deserialize body payload to json
                JsonNode data;
                string jsonString = string.Empty;
                try
                {
                    data = JsonSerializer.Deserialize<JsonNode>(req.Body);
                    jsonString = data.ToString() ?? string.Empty;
                } 
                catch (JsonException e)
                {

                    log.LogError(e.ToString());
                    contentResult.Content = JsonErrorStr(e.ToString());
                    contentResult.StatusCode = StatusCodes.Status400BadRequest;
                    return contentResult;
                } // .catch

                string logJsonString = TruncateStrForLog(data.ToJsonString(), maxLengthForLog);
                log.LogInformation("{prefix}ProcessMessage bundle received: {logJsonString}", prefix, logJsonString);

                log.LogInformation("flagProcessMessageFunctionSkipValidate {flagProcessMessageFunctionSkipValidate} ", flagProcessMessageFunctionSkipValidate);


                bool isValid;
                if (flagProcessMessageFunctionSkipValidate)
                {
                    isValid = true;
                }
                else
                {
                    var location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle/$validate");
                    string cleanedBearerToken = CleanBearerToken(req.Headers[authorizationKeyName]);
                    DateTime startFHIRValidation = DateTime.Now;
                    PostContentBundleResult validateReportingBundleResult = await PostContentBundle(configuration, jsonString, location, cleanedBearerToken, log);
                    TimeSpan durationFHIRValidation = DateTime.Now - startFHIRValidation;
                    string logLogDetail = TruncateStrForLog(validateReportingBundleResult.JsonString, maxLengthForLog);
                    double ms = durationFHIRValidation.Milliseconds;
                    log.LogInformation("{prefix}ProcessMessage FHIR validation done with result: {logLogDetail}", prefix, logLogDetail);
                    log.LogInformation("{prefix}ProcessMessage FHIR validation run duration ms: {ms}", prefix, ms);
                    isValid = !validateReportingBundleResult.JsonString.Contains("\"severity\":\"error\"");
                }

                if (isValid)
                {                   
                    var location = new Uri($"{configuration["BaseFhirUrl"]}/Bundle");                                   
                    JsonNode messageNode = data;
                    string cleanedBearerToken = CleanBearerToken(req.Headers[authorizationKeyName]);
                    PostContentBundleResult postResult = await PostContentBundle(configuration, messageNode.ToJsonString(), location, cleanedBearerToken, log);
                    data = JsonNode.Parse(postResult.JsonString);
                    contentResult.Content = data.ToJsonString();
                    contentResult.StatusCode = StatusCodes.Status201Created;
                    return contentResult;
                }
                else
                {              
                    contentResult.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return contentResult;
                }
            }
            catch (Exception e)
            {
                if (e is HttpRequestException httpException) // exception returned from the FHIR server call
                {
                    contentResult.Content = JsonErrorStr("http error");
                    contentResult.StatusCode = (int?)httpException.StatusCode;
                }
                else // something else (exception) happened
                {
                    contentResult.Content = JsonErrorStr("unexpected condition was encountered");
                    contentResult.StatusCode = StatusCodes.Status500InternalServerError;
                }
                log.LogError(e.ToString());

                return contentResult;

            } // .catch
            finally
            {
                TimeSpan durationProcessMessage = DateTime.Now - startProcessMessage;
                double durationPmMs = durationProcessMessage.Milliseconds;
                log.LogInformation("{prefix}ProcessMessage total run duration ms: {durationPmMs}", prefix, durationPmMs);
            }

        } // .run

        private async Task<PostContentBundleResult> PostContentBundle(IConfiguration configuration, string bundleJson, Uri location, string bearerToken, ILogger log)
        {
            string prefix = "ProcessMessage: ";
            PostContentBundleResult postContentResponse;
            string locAbs = location.AbsoluteUri;
            log.LogInformation("{prefix}ProcessMessage, Post Bundle to FHIR server endpoint: {locAbs}", prefix, locAbs);

            using (HttpClient client = httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, location) { Content = new StringContent(bundleJson, System.Text.Encoding.UTF8, "application/json") })
            {

                //passthrough the cleaned auth bearer token used
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

             

                client.Timeout = TimeSpan.FromMinutes(configuration.GetValue<double>("FunctionProcessMessage:TimeOut", 5));

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                bool successCode = response.IsSuccessStatusCode;

                log.LogInformation("{prefix}ProcessMessage, PostContentBundle received from FHIR server http response status code success: {successCod}", prefix, successCode);

                postContentResponse = new PostContentBundleResult() { StatusCode = response.StatusCode, JsonString = jsonString };
            }

            return postContentResponse;
        }

        private static string CleanBearerToken(string bearerToken)
        {
            Regex regexString = new Regex("[^a-zA-Z0-9\\.\\-_ ]",RegexOptions.None, TimeSpan.FromMilliseconds(100));
            string cleanedBearerToken = regexString.Replace(bearerToken, "");
            cleanedBearerToken = cleanedBearerToken.Replace("Bearer ", "");

            return cleanedBearerToken;
        }

        private static string JsonErrorStr(string errorMessage)
        {
            return new JsonObject
            {
                ["error"] = errorMessage.ToLower(),
            }.ToJsonString();
        } // .JsonErrorStr

        private static string TruncateStrForLog(string jsonString, int maxLen)
        {
            return jsonString.Length > maxLen ? jsonString.Substring(0, maxLen) + "..." : jsonString;
        } // .TruncateStrForLog

        [FunctionName("Health")]
        public IActionResult RunHealthCheck(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            ContentResult contentResult = new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int?)HttpStatusCode.OK
            };

            return contentResult;
        }

    } // .class 
} // .namespace 
