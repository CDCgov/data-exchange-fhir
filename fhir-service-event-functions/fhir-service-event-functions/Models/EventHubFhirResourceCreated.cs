using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Models
{
    internal class EventHubFhirResourceCreated
    {
        public string id;
        public string topic;
        public string subject;
        public EventHubFhirResourceCreatedData data;
        public string eventType;
        public string dataVersion;
        public string metadataVersion;
        public string eventTime;

    }
}
