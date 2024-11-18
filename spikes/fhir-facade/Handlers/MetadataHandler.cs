using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace OneCDPFHIRFacade.Handlers
{
    public class MetadataHandler
    {
        public void PrintToConsole(Patient thePatient, Dictionary<string, Patient> myPatient)
        {
            Console.WriteLine("Begining of printPatientToConsole");
            try
            {
                string jsonString = thePatient.ToJson();
                myPatient.Add(thePatient.Id, thePatient);
                Console.WriteLine("Received the bundle");
                Console.WriteLine(jsonString);
                Console.WriteLine("End of printPatientToConsole");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void savePatientToS3(Patient thePatient)
        {
            MetadataCollection omd = new MetadataCollection();
            NameValueCollection prop = new NameValueCollection();
            string clientRegion = "us-east-1";
            string bucketName = "dexfhirbucket";
            Uuid anID = Uuid.Generate();
            string filename = anID.ToString()!;
            string aFile = filename + ".json";
            try
            {
                using (FileStream fs = new FileStream("fhir.properties", FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line!.Split('=');
                        if (parts.Length == 2)
                        {
                            prop[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
                if (prop.Get("AccessKey") == null || prop["SecretToken"] == null)
                {
                    throw new Exception("AccessKey or SecretToken not found.");
                }

                // Fetching properties (Accesskey, SecretToken)
                string accessKey = prop["Accesskey"]!;
                string secretKey = prop["SecretToken"]!;

                // Convert the FHIR object to a JSON string (using Newtonsoft.Json as an example)
                string jsonString = JsonConvert.SerializeObject(thePatient, Formatting.Indented);
                Console.WriteLine("Before GSON");

                // Write the JSON string to a file
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), aFile);
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(jsonString);
                }

                // Set up AWS credentials and S3 client
                var creds = new BasicAWSCredentials(accessKey, secretKey);
                var s3Client = new AmazonS3Client(creds, RegionEndpoint.GetBySystemName(clientRegion));

                // Upload the file to S3
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = aFile,
                    FilePath = filePath
                };

                s3Client.PutObjectAsync(putRequest);

                Console.WriteLine("File uploaded to S3 successfully.");

            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: " + exp.Message);
                Console.WriteLine(exp.StackTrace);
            }
        }
    }
}
