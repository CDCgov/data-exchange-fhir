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

                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        string inputFileName = fileInfo.Name;
                        double fileSizeInMb = fileInfo.Length / (1024.0 * 1024.0);
                        string subFolder = null!;

                        if (fileSizeInMb > 4 && fileSizeInMb < 6)
                        {
                            subFolder = "5mb";
                        }
                        else if (fileSizeInMb > 9 && fileSizeInMb < 11)
                        {
                            subFolder = "10mb";
                        }
                        else if (fileSizeInMb > 24 && fileSizeInMb < 26)
                        {
                            subFolder = "25mb";
                        }
                        else if (fileSizeInMb > 49 && fileSizeInMb < 51)
                        {
                            subFolder = "50mb";
                        }

                        if (subFolder != null)
                        {
                            string destinationFolder = Path.Combine(outputFolder, subFolder);
                            Directory.CreateDirectory(destinationFolder); // Ensure directory exists

                            outputFile = Path.Combine(destinationFolder, inputFileName);
                            File.Copy(files[i], outputFile, true);

                            Console.WriteLine($"Copied {inputFileName} to {outputFile}");
                        }
                    }

                }
                else
                {
                    Console.WriteLine("ERROR: Source folder does not exist.");
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
