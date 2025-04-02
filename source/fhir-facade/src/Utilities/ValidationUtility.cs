using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Validator = Firely.Fhir.Validation.Validator;


namespace OneCDPFHIRFacade.Utilities
{
    public class ValidationUtility
    {

        public string ValidateBundle(Bundle bundle)
        {
            var packagePath = @"C:\Dev\GitHub\CDCOneCDP\data-exchange-fhir\source\fhir-facade";  // Ensure VSAC package is placed here
            var packageResolver = new DirectorySource(packagePath, new DirectorySourceSettings { IncludeSubDirectories = true });


            var resourceResolver = new CachedResolver(packageResolver);
            var terminologyService = new LocalTerminologyService(resourceResolver);

            var validator = new Validator(resourceResolver, terminologyService);

            var profile = "http://hl7.org/fhir/us/ecr/StructureDefinition/eicr-document-bundle";
            var result = validator.Validate(bundle, profile);

            return result.Success.ToString();


        }

    }
}
