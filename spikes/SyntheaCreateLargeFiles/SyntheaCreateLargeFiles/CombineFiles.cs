namespace SyntheaCreateLargeFiles
{
    internal class CombineFiles
    {
        public CombineFiles(string folderPath, string outputFolder, string fileName)
        {
            try
            {
                outputFolder = $"{outputFolder}300mb";

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
                double maxSizeMB = 290.0;
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

                            if (!string.IsNullOrEmpty(fileContent))
                            {
                                // Find the index of the first occurrence of `"resourceType": "Bundle"`
                                int insertIndex = fileContent.IndexOf("\"resourceType\": \"Bundle\"");
                                if (insertIndex != -1)
                                {
                                    int insertPosition = insertIndex + "\"resourceType\": \"Bundle\"".Length;

                                    // Insert `"id": "125",` right after the first `"resourceType": "Bundle"`
                                    string modifiedContent = fileContent.Insert(insertPosition, ",\n  \"id\": \"125\"");

                                    fileContent = modifiedContent;
                                }
                            }

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
