using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Models
{
    internal class EventHubFhirResourceCreatedData
    {
        public string resourceType;
        public string resourceFhirAccount;
        public string resourceFhirId;
        public string resourceVersionId;
    }
}
