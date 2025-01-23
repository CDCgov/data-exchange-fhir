namespace OneCDPFHIRFacade.Config
{
    public static class LocalFileStorageConfig
    {
        public const string KeyName = "FileSettings";
        public static string? LocalDevFolder { get; set; }

        public static bool UseLocalDevFolder { get; set; }

        public static void Initialize(IConfiguration configuration)
        {
            var section = configuration.GetSection(KeyName);
            LocalDevFolder = section.GetValue<string>("LocalDevFolder");
            if (configuration.GetValue<string>("RunEnvironment") == "Local")
            {
                UseLocalDevFolder = true;
            }
            else
            {
                UseLocalDevFolder = false;
            }
        }
    }
}
