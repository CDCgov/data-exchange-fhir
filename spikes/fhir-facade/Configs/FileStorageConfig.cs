namespace FirelyApiApp.Configs
{
    public class FileStorageConfig
    {
        public static string KeyName = "FileSettings";
        public bool UseLocalDev { get; set; }
        public required string LocalDevFolder { get; set; }
    }
}
