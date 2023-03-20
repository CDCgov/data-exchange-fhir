using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharedcode_fhir_service_function.Models
{
    public class FhirResourceCreatedData
    {
        public string resourceType;
        public string resourceFhirAccount;
        public string resourceFhirId;
        public string resourceVersionId;
    }
}
