using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;

namespace AzureAppConfigSampleFunction
{
    public class AzureAppConfigShowMessage
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _configurationRefresher;

        public AzureAppConfigShowMessage(IConfiguration configuration, IConfigurationRefresherProvider refresherProvider)
        {
            _configuration = configuration;
            _configurationRefresher = refresherProvider.Refreshers.First();
        }

        // Uncomment the FunctionName attribute below to enable the function
        //[FunctionName("AzureAppConfigShowMessage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _configurationRefresher.TryRefreshAsync();

            // Read the key value in App Configuration.
            string key = "TestApp:Settings:Message";
            string message = _configuration[key];

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult($"Please create a key-value with the key '{key}' in Azure App Configuration.");
        }
    }
}
