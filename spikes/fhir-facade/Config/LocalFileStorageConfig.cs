namespace OneCDPFHIRFacade.Config
{
    public static class LocalFileStorageConfig
    {
        public static string KeyName = "FileSettings";
        public static bool UseLocalDevFolder { get; set; }
        public static string? LocalDevFolder { get; set; }

        public static void Initialize(IConfiguration configuration)
        {
            var section = configuration.GetSection(KeyName);
            UseLocalDevFolder = section.GetValue<bool>("UseLocalDevFolder");
            LocalDevFolder = section.GetValue<string>("LocalDevFolder");
        }
    }
}
