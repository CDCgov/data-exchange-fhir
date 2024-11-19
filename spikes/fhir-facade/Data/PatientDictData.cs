using Hl7.Fhir.Model;
using System.Collections.Concurrent;

namespace OneCDPFHIRFacade.Data
{
    public static class PatientDictData
    {
        public static ConcurrentDictionary<string, Patient> PatientDictionary { get; set; }
    }
}
