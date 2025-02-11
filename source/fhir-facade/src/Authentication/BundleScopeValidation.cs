using Hl7.Fhir.Model;
using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;
namespace OneCDPFHIRFacade.Authentication
{
    public class BundleScopeValidation
    {
        private readonly LoggingUtility loggingUtility;
        private readonly Bundle bundle;
        public BundleScopeValidation(Bundle bundle, LoggingUtility loggingUtility)
        {
            this.bundle = bundle;
            this.loggingUtility = loggingUtility;
        }

        public string[] GetBundleProfile()
        {
            try
            {
                var bundleMetaProfile = bundle.Meta.Profile;
                string[] metaProfile = bundleMetaProfile.ToArray();
                return metaProfile;
            }
            catch (Exception)
            {
                Console.WriteLine("Meta profile not in Bundle");
                return [];
            }
        }
        public async Task<bool> IsBundleProfileMatchScope()
        {
            if (AwsConfig.ScopeClaim == null || AwsConfig.ScopeClaim.Length == 0)
            {
                Console.WriteLine("Missing or empty 'scope' claim.");
                await loggingUtility.Logging("Missing or empty 'scope' claim.", "Scope Validator");
                await loggingUtility.SaveLogS3("ScopeError");
                return false;
            }
            bool valid = false;
            string[] bundleMetaProfile = GetBundleProfile();
            foreach (var scope in AwsConfig.ScopeClaim)
            {
                foreach (var item in bundleMetaProfile)
                {
                    if (item.Contains(scope))
                    {
                        return true;
                    }
                }

            }
            return valid;
        }
    }
}
