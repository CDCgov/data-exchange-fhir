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
            var scopeClaim = AwsConfig.ScopeClaim;
            if (scopeClaim == null || scopeClaim.Length == 0)
            {
                Console.WriteLine("Missing or empty 'scope' claim.");
                await loggingUtility.Logging("Missing or empty 'scope' claim.");
                await loggingUtility.SaveLogS3("ScopeError");
                return false;
            }
            bool valid = false;
            string[] bundleMetaProfile = GetBundleProfile();

            foreach (var item in bundleMetaProfile)
            {
                int pos = item.LastIndexOf("/") + 1;
                string getProfile = item.Substring(pos, item.Length - pos);
                foreach (var scope in scopeClaim)
                {
                    if (scope.Contains("stream"))
                    {
                        string[] scopeSplit = scope.Split(new string[] { "/" }, StringSplitOptions.None);
                        string currentScope = scopeSplit[1];
                        if (currentScope.Equals(getProfile))
                        {
                            return true;
                        }
                    }
                }
            }
            return valid;
        }
    }
}
