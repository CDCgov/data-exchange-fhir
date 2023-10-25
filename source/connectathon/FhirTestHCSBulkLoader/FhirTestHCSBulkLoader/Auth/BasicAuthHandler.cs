using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using static FhirTestHCSBulkLoader.Config.LogConfig;


namespace FhirTestHCSBulkLoader.Auth
{
    internal class BasicAuthHandler : IAuthHandler
    {
        public async Task<string> GetFhirServerToken(IConfiguration config, HttpClient httpClient)
        {
            string token;

            if (!string.IsNullOrEmpty(config["ClientId"]) && !string.IsNullOrEmpty(config["ClientSecret"]))
            {
                var dict = new Dictionary<string, string>();
                dict.Add("grant_type", "Client_Credentials");
                dict.Add("client_id", config["ClientId"]);
                dict.Add("client_secret", config["ClientSecret"]);

                using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{config["BaseFhirUrl"]}/auth") { Content = new FormUrlEncodedContent(dict) })
                {
                    Log("Authenticating with Fhir Server", LogType.Info);

                    var tokenResponse = await httpClient.SendAsync(tokenRequest);

                    //tokenResponse.EnsureSuccessStatusCode();

                    var result = await tokenResponse.Content.ReadFromJsonAsync<AuthTokenResult>();

                    Log("Access Token Generated Succesfully", LogType.Good);

                    token = result!.access_token;
                }
            }
            else
            {
                Log("Using Cached Bearer Token", LogType.Info);
                token = config["BearerToken"];
            }

            return token;
        }

    }
}
