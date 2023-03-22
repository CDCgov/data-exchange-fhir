using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace sharedcode_fhir_service_function.Models
{
    public class PostContentBundleResult
    {
        public string JsonString { get; set; } 
        public HttpStatusCode StatusCode { get; set; }
    }
}
