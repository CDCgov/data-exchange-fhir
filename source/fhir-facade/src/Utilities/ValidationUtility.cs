using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Validator = Firely.Fhir.Validation.Validator;

namespace OneCDPFHIRFacade.Utilities
{
    public class ValidationUtility
    {

        public bool ValidateBundle(Bundle bundle)
        {
            // Path to your local FHIR package cache
            var packageRoot = @"C:\Users\AC56\.fhir\packages";
            string[] packageDirs = new string[]
            {
                Path.Combine(packageRoot, "hl7.fhir.r4.core#4.0.1"),  // Core FHIR R4 package
                Path.Combine(packageRoot, "hl7.fhir.us.ecr#latest")   // US ECR Package
            };

            // Load only the required directories
            var resolvers = packageDirs
                .Where(Directory.Exists)
                .Select(dir => new DirectorySource(dir, new DirectorySourceSettings { IncludeSubDirectories = true }))
                .ToArray();

            var resourceResolver = new CachedResolver(new MultiResolver(resolvers));
            var terminologyService = new LocalTerminologyService(resourceResolver);
            var validator = new Validator(resourceResolver, terminologyService);

            // Validate Bundle
            var profile = "http://hl7.org/fhir/us/ecr/StructureDefinition/eicr-document-bundle";
            var result = validator.Validate(bundle, profile);
            return result.Success;

        }
    }
}
