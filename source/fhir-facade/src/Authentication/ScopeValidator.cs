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

        public bool Validate(string? scopeClaim)
        {
            if (string.IsNullOrWhiteSpace(scopeClaim))
            {
                Console.WriteLine("Missing or empty 'scope' claim.");
                return false;
            }

            var scopes = scopeClaim.Split(' ');
            var isValid = HasValidScope(scopes);

            Console.WriteLine($"Scope claim: {scopeClaim}, Valid scope: {isValid}");
            return isValid;
        }
    }
}

