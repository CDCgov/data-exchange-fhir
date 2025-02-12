namespace SyntheaCreateLargeFiles
{
    internal class CombineFiles
    {
        public CombineFiles(string folderPath, string outputFolder, string fileName)
        {
            try
            {
                outputFolder = $"{outputFolder}50mb";

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine("Folder does not exist.");
                    return;
                }

                string[] files = Directory.GetFiles(folderPath);
                int fileIndex = 0;
                int fileCount = 0;
                double maxSizeMB = 50.0;
                List<string> jsonObjects = new List<string>();

                while (fileIndex < files.Length && fileCount < 20)
                {
                    string outputFile = Path.Combine(outputFolder, $"{fileName}{fileCount}.json");
                    using (StreamWriter writer = new StreamWriter(outputFile))
                    {
                        double currentFileSizeMB = 0;

                        while (fileIndex < files.Length && currentFileSizeMB < maxSizeMB)
                        {
                            string fileContent = File.ReadAllText(files[fileIndex]).Trim();
                            FileInfo inputFile = new FileInfo(files[fileIndex]);
                            double inputFileSizeMB = inputFile.Length / (1024.0 * 1024.0);

                            if (jsonObjects.Count > 0) writer.WriteLine(",");
                            jsonObjects.Add(fileContent);
                            writer.Write(fileContent);
                            currentFileSizeMB += inputFileSizeMB;

                            fileIndex++;
                        }
                    }
                    fileCount++;
                }
                Console.WriteLine($"Merged JSON files created in folder: {outputFolder}");
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
