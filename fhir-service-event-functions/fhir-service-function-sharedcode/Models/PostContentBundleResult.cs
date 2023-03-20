using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Models
{
    public class PostContentBundleResult
    {
        public string JsonString { get; set; } 
        public HttpStatusCode StatusCode { get; set; }
    }
}
