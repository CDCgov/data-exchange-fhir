namespace OneCDPFHIRFacade.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        // Constructor with a custom message
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }

        // Constructor with an optional inner exception for chaining
        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor that accepts the ID of the missing resource
        public ResourceNotFoundException(object resourceId)
            : base($"Resource with ID '{resourceId}' was not found.")
        {
        }
    }
}
