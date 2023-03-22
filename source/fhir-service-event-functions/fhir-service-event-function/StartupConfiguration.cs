using fhir_service_event_functions.Config;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(StartupConfiguration))]
namespace fhir_service_event_functions.Config
{
    public class StartupConfiguration : FunctionsStartup
    {

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("FunctionappAppconfigConnectionstring");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(cs);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // http client factory
            builder.Services.AddHttpClient("RetryClientFactory")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy(3));

        }

        /// <summary>
        /// Retry policy with exponential backoff using Polly
        /// </summary>
        /// <param name="retries">Number of retries</param>
        /// <returns>Retry policy with exponential backoff</returns>
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retries)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.NotFound)
                .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized)
                .WaitAndRetryAsync(retries, sleepDuration => TimeSpan.FromSeconds(Math.Pow(2, sleepDuration)));
        }
    }
}
