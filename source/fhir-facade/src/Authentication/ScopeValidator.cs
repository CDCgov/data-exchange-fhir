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
        public async Task<bool> Validate(string? scopeClaim, string[] requiredScopes)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var loggingUtility = scope.ServiceProvider.GetRequiredService<LoggingUtility>();
                if (string.IsNullOrEmpty(scopeClaim))
                {
                    Console.WriteLine("Missing or empty 'scope' claim.");
                    await loggingUtility.Logging("Missing or empty 'scope' claim.");
                    await loggingUtility.SaveLogS3("ScopeError");
                    return false;
                }

                var scopes = scopeClaim.Split(' ');
                var isValid = requiredScopes.Any(required => scopes.Any(scope => scope.StartsWith(required)));

                Console.WriteLine($"Scope claim: {scopeClaim}, Valid scope: {isValid}");
                await loggingUtility.Logging($"Scope claim: {scopeClaim}, Valid scope: {isValid}");
                return isValid;
            }
        }
    }
}

