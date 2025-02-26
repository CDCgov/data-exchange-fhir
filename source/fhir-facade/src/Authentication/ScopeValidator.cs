using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace OneCDPFHIRFacade.Authentication
{
    public class ScopeValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopeValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // check the scope claim from the JTW token are valid scopes from config scopes
        public async Task<bool> Validate(string scopeClaimString)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var loggingUtility = scope.ServiceProvider.GetRequiredService<LoggingUtility>();
                string[] sp = AwsConfig.ScopeClaim!;
                if (sp == null && sp!.Length == 0)
                {
                    Console.WriteLine("Missing or empty 'scope' claim.");
                    await loggingUtility.Logging("Missing or empty 'scope' claim.");
                    await loggingUtility.SaveLogS3("ScopeError");
                    return false;
                }

                foreach (var item in sp)
                {
                    if (item.Contains("system"))
                    {
                        var isValid = AwsConfig.ClientScope!.Any(existingScope => existingScope.Equals(item, StringComparison.OrdinalIgnoreCase));
                        Console.WriteLine($"Scope claim: {scopeClaimString}, Valid scope: {isValid}");
                        await loggingUtility.Logging($"Scope claim: {scopeClaimString}, Valid scope: {isValid}");
                        return isValid;
                    }
                }

                return false;
            }
        }
    }
}

