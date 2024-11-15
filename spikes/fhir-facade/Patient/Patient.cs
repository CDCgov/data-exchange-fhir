using Hl7.Fhir.Model;
namespace OneCDPFHIRFacade.Patient
{
    public class Patient
    {
        public static HumanName.NameUse Use = HumanName.NameUse.Official;
        public static string[] Prefix = new string[] { "Mr" };
        public static string[] Given = new string[] { "Sam" };
        public static string Family = "Fhirman";

    }
}
