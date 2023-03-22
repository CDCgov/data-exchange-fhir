using Microsoft.Extensions.Configuration;
using sharedcode_fhir_service_function.Models;
using System.Net.Http.Json;

namespace sharedcode_fhir_service_function.Util
{
    public class FhirServiceUtils
    {
        /// <summary>
        /// Get the service principle to access to the azure fhir server
        /// </summary>
        /// <param name="authConfig">The environment specfic authConfig</param>
        /// <param name="httpClient">The httpClient to use to get the fhir token</param>
        public static async Task<string> GetFhirServerToken(IConfiguration configuration, HttpClient httpClient)
        {
            string token;

            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "Client_Credentials");
            dict.Add("client_id", configuration["ClientId"]);
            dict.Add("client_secret", configuration["ClientSecret"]);

            using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{configuration["FhirUrl"]}/auth") { Content = new FormUrlEncodedContent(dict) })
            {
                tokenRequest.Headers.Add("Ocp-Apim-Subscription-Key", configuration["OcpApimSubscriptionKey"]);

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
