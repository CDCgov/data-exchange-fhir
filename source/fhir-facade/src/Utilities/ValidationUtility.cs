using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using System.Diagnostics.CodeAnalysis;
using Validator = Firely.Fhir.Validation.Validator;

namespace OneCDPFHIRFacade.Utilities
{
    public class ValidationUtility
    {
        [SuppressMessage("SonarQube", "csharpsquid:S5332", Justification = "We are not access this, we are ")]

        public string ValidateBundle(Bundle bundle)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory());
            var packagePath = @$"{filePath}/ProfilePackages";
            var packageResolver = new DirectorySource(packagePath, new DirectorySourceSettings { IncludeSubDirectories = true });


            var resourceResolver = new CachedResolver(packageResolver);
            var terminologyService = new LocalTerminologyService(resourceResolver);

            var validator = new Validator(resourceResolver, terminologyService);

            var profile = "http://hl7.org/fhir/us/ecr/StructureDefinition/eicr-document-bundle";
            var result = validator.Validate(bundle, profile);
            // Collect all validation messages
            var errorMessages = result.Issue.Select(issue =>
                $"{issue.Severity}: {issue.Code} - {issue.Details?.Text} (Location: {string.Join(", ", issue.Location)})"
            ).ToList();

            string resultString = $"Bundle Validation Result: {result.Success} {string.Join(" ", errorMessages)}";

            return resultString;
        }

    }
}
