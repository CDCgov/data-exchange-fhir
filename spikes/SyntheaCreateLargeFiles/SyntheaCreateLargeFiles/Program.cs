namespace SyntheaCreateLargeFiles
{
    class Program
    {
        static void Main()
        {
            string folderPath = @"C:\Users\AC56\Downloads\synthea\output\fhir"; // Change this to your folder path
            string outputFolder = @"C:\Dev\SyntheaCreateLargeFiles\SyntheaCreateLargeFiles\"; // Use a safe directory
            DateTime dateTime = DateTime.Now;
            string fileName = dateTime.ToString("mmdd");

            CombineFiles combineFiles = new CombineFiles(folderPath, outputFolder, fileName);
            LargeFileFromSynthea largeFileFromSynthea = new LargeFileFromSynthea(folderPath, outputFolder, fileName);
        }
    }
}
