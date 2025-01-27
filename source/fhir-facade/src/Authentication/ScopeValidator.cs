namespace OneCDPFHIRFacade.Authentication
{
    public class ScopeValidator(params string[] requiredSuffixes)
    {

        private readonly string[] _requiredSuffixes = requiredSuffixes;

        public bool HasValidScope(IEnumerable<string> scopes)
        {
            // Check if any scope matches the required suffixes
            return scopes.Any(scope => _requiredSuffixes.Any(suffix => scope.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> Validate(string? scopeClaim)
        {
            LoggerService loggerService = new LoggerService();
            if (string.IsNullOrWhiteSpace(scopeClaim))
            {
                Console.WriteLine("Missing or empty 'scope' claim.");
                await loggerService.LogData("Missing or empty 'scope' claim.", "Scope Validator");
                //Todo: add logs to S3
                return false;
            }

            var scopes = scopeClaim.Split(' ');
            var isValid = HasValidScope(scopes);

            Console.WriteLine($"Scope claim: {scopeClaim}, Valid scope: {isValid}");
            await loggerService.LogData($"Scope claim: {scopeClaim}, Valid scope: {isValid}", "Scope Validator");
            //Todo: add logs to S3 
            return isValid;
        }
    }
}

