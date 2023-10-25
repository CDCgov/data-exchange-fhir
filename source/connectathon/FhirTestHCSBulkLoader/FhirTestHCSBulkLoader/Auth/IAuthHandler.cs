using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirTestHCSBulkLoader.Auth
{
    internal interface IAuthHandler
    {
        Task<string> GetFhirServerToken(IConfiguration config, HttpClient httpClient);

    }
}
