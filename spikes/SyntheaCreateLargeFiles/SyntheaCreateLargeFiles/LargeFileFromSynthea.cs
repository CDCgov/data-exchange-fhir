using Newtonsoft.Json.Linq; // Ensure you have Newtonsoft.Json installed

namespace SyntheaCreateLargeFiles
{
    internal class LargeFileFromSynthea
    {
        public LargeFileFromSynthea(string folderPath, string outputFolder, string fileName)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath, "*.json"); // Get all JSON files

                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        string inputFileName = fileInfo.Name;
                        double fileSizeInMb = fileInfo.Length / (1024.0 * 1024.0);
                        string subFolder = null!;

                        if (fileSizeInMb > 4 && fileSizeInMb < 6)
                        {
                            subFolder = "5mb";
                        }
                        //else if (fileSizeInMb > 9 && fileSizeInMb < 11)
                        //{
                        //    subFolder = "10mb";
                        //}
                        //else if (fileSizeInMb > 24 && fileSizeInMb < 26)
                        //{
                        //    subFolder = "25mb";
                        //}
                        //else if (fileSizeInMb > 49 && fileSizeInMb < 51)
                        //{
                        //    subFolder = "50mb";
                        //}

                        if (subFolder != null)
                        {
                            string destinationFolder = Path.Combine(outputFolder, subFolder);
                            Directory.CreateDirectory(destinationFolder); // Ensure directory exists

                            string outputFile = Path.Combine(destinationFolder, inputFileName);

                            // Read and modify JSON
                            string jsonContent = File.ReadAllText(file);
                            JObject jsonObject = JObject.Parse(jsonContent);

                            if (jsonObject["resourceType"]?.ToString() == "Bundle")
                            {
                                jsonObject["id"] = "123"; // Add id field
                                jsonObject["meta"] = new JObject
                                {
                                    ["profile"] = new JArray("http://hl7.org/fhir/us/ecr/StructureDefinition/eicr-document-bundle")
                                };
                            }

                            File.WriteAllText(outputFile, jsonObject.ToString());

                            Console.WriteLine($"Copied and modified {inputFileName} to {outputFile}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Source folder does not exist.");
                }
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
