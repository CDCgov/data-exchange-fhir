namespace SyntheaCreateLargeFiles
{
    internal class LargeFileFromSynthea
    {
        public LargeFileFromSynthea(string folderPath, string outputFolder, string fileName)
        {

            try
            {
                string outputFile = outputFolder;

                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath, "*.json"); // Get all JSON files

                    for (int i = 0; i < files.Length - 1; i++)
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        string inputFileName = fileInfo.Name;
                        double fileSizeInMb = fileInfo.Length / (1024.0 * 1024.0);

                        if (fileSizeInMb > 4 && fileSizeInMb < 6)
                        {
                            outputFile = Path.Combine($"{outputFolder}5mb", inputFileName);

                            File.Copy(files[i], $"outputFile", true);
                        }
                        else if (fileSizeInMb > 9 && fileSizeInMb < 11)
                        {
                            outputFile = Path.Combine($"{outputFolder}10mb", inputFileName);

                            File.Copy(files[i], $"outputFile", true);
                        }
                        else if (fileSizeInMb > 24 && fileSizeInMb < 26)
                        {
                            outputFile = Path.Combine($"{outputFolder}25mb", inputFileName);

                            File.Copy(files[i], $"outputFile", true);
                        }
                        else if (fileSizeInMb > 49 && fileSizeInMb < 51)
                        {
                            outputFile = Path.Combine($"{outputFolder}50mb", inputFileName);

                            File.Copy(files[i], $"outputFile", true);
                        }

                    }

                }
                Console.WriteLine($"Finish writing file to {outputFile}");

            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("ERROR: Access denied. Try running as Administrator or using a different output folder.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
