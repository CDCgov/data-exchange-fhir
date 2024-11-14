namespace fhirfacade.Configs
{
    public class FileStorageConfig
    {
        public static string KeyName = "FileSettings";
        public static bool UseLocalDevFolder { get; set; }
        public static string LocalDevFolder { get; set; } = "";
    }
}
