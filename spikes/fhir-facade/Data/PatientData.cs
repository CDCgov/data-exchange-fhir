using Hl7.Fhir.Model;

namespace OneCDPFHIRFacade.Data
{
    public class PatientData
    {
        public PatientData(string id)
        {
            Patient pat1 = new Patient();
            if (PatientDictData.PatientDictionary[id] != null)
            {
                Guid generatedId = Guid.NewGuid();
                pat1.Id = id + generatedId;
            }
            pat1.Id = id;
            Identifier identifier = new Identifier
            {
                System = Environment.GetEnvironmentVariable("PATIENT_IDENTIFIER_SYSTEM"),
                Value = Environment.GetEnvironmentVariable("PATIENT_IDENTIFIER_VALUE")
            };
            pat1.Identifier.Add(identifier);
            HumanName patient1 = new HumanName()
            {
                Family = "Simpson",
                Given = ["Homer", " J."]
            };
            pat1.Name.Add(patient1);

            PatientDictData.PatientDictionary.GetOrAdd(id, pat1);
        }
    }


}
