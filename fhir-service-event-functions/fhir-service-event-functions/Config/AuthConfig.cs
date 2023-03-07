using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Config
{
    public class AuthConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string FhirUrl { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public string Scope { get; set; }
        public string ConnectionStringServiceBus { get; set; }

        public static AuthConfig ReadFromEnvironmentVariables()
        {
            AuthConfig config = new AuthConfig();
            config.ClientId = Environment.GetEnvironmentVariable("ClientId");
            config.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
            config.FhirUrl = Environment.GetEnvironmentVariable("FhirUrl");
            config.OcpApimSubscriptionKey = Environment.GetEnvironmentVariable("OcpApimSubscriptionKey");
            config.Scope = Environment.GetEnvironmentVariable("Scope");
            config.ConnectionStringServiceBus = Environment.GetEnvironmentVariable("fhireventqueue_servicebusnsfhir_connectionstring");
            return config;
        }
        public static AuthConfig ReadFromJsonFile(string path)
        {
            IConfigurationRoot Configuration;

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path);

            Configuration = builder.Build();
            return Configuration.Get<AuthConfig>();
        }


    }
}

