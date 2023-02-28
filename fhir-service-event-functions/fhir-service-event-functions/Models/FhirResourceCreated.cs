using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Models
{
    public class FhirResourceCreated
    {
        public string id;
        public string topic;
        public string subject;
        public FhirResourceCreatedData data;
        public string eventType;
        public string dataVersion;
        public string metadataVersion;
        public string eventTime;

    }
}
