using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly.Extensions.Http;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;
using fhir_service_event_functions.Config;

[assembly: FunctionsStartup(typeof(StartupConfiguration))]
namespace fhir_service_event_functions.Config
{
    public class StartupConfiguration : FunctionsStartup
    {
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
