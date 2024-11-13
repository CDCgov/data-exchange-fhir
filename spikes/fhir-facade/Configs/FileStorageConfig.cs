namespace FirelyApiApp.Configs
{
    public class FileStorageConfig
    {
        public static string KeyName = "FileSettings";
        public bool UseLocalDevFolder { get; set; }
        public required string LocalDevFolder { get; set; }
    }
}
