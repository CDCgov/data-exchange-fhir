using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhir_service_event_functions.Config
{
    public class FeatureFlagConfig
    {

        public bool FhirResourceCreatedExportFunctionFlatten { get; set; }
        public bool FhirResourceCreatedExportFunctionUnbundle { get; set; }

        public static FeatureFlagConfig ReadFromEnvironmentVariables()
        {
            FeatureFlagConfig config = new FeatureFlagConfig();
            config.FhirResourceCreatedExportFunctionFlatten = bool.Parse(Environment.GetEnvironmentVariable("FhirResourceCreatedExportFunctionFlatten"));
            config.FhirResourceCreatedExportFunctionUnbundle = bool.Parse(Environment.GetEnvironmentVariable("FhirResourceCreatedExportFunctionUnbundle"));

            return config;
        }
    }


}
