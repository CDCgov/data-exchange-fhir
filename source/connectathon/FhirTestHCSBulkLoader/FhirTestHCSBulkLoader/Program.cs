using FhirTestHCSBulkLoader.Auth;
using FhirTestHCSBulkLoader.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static FhirTestHCSBulkLoader.Config.LogConfig;

namespace CDC.DEX.FHIR
{

    public class FhirTestHCSBulkLoaderMain
    {

        // TODO: change config handling depending on final hosting solution
        public static IConfiguration config;

        // ------ VARIABLES TO CHANGE

        // Root path to look for existing files, if none exist, will generate a new set of HCS Bundles
        //const string _root_path = @"C:\FhirTestHCSBulkLoader";

        // Base Fhir URL to hit
        //const string _baseFhirUrl = @"https://apidevfhir.cdc.gov";
        //const string _baseFhirUrl = @"https://cdc-dmi-nb-test-apim.azure-api.net";

        // Use either Client ID/Secret or a pregenerated bearer token, will prioritize client id/secret, if blank will use bearer token
        //const string clientId = "";
        //const string clientSecret = "";
        //const string _bearer_token = "";

        //The log prefix
        //const string _log_prefix = "FHIR.HCS.BulkLoader";

        // ------ END VARIABLES TO CHANGE


        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Random random = new Random();
        private static IAuthHandler authHandler;
        private static IDataGenerator dataGenerator;

        static void Main(string[] args)
        {
            try
            {
                Log("BEGIN", LogType.Good);
                Initialize();
                Run();
            }
            catch (Exception e)
            {
                Log("EXCEPTION", LogType.Bad);
                Log(e.ToString(), LogType.Bad);
            }
            finally
            {
                Log("Finished (Some queued http calls may still be running)", LogType.Good);
                Console.WriteLine("Press any key to exit");
                Console.Read();
            }
        }

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("config_internal.json", optional: false);
                .AddJsonFile("config_cdc.json", optional: false);

            config = builder.Build();

            authHandler = new BasicAuthHandler();
            authHandler = new SmartAuthHandler();

            //TODO: REMOVE THIS, TESTING ONLY
            //IAuthHandler REMOVETHIS = new SmartAuthHandler();
            //REMOVETHIS.GetFhirServerToken(config, _httpClient);

            //TODO: change Data Generator and options
            dataGenerator = new BasicHCSDataGenerator(random);
        }

        private static void Run()
        {

            string path = config["RootPath"];
            // CHECK FOR EXISTING FILES
            string[] files = Directory.GetFiles(path);
            if (files.Length > 0)
            {
                Log("Existing files found in root folder, sending those to process-message", LogType.Info);
            }
            else
            {
                // NO EXISTING FILES IN ROOT FOLDER, MAKING SUBFOLDER AND GENERATING FILES
                Log("No existing files found in root folder, will generate new HCS Bundles", LogType.Info);

                //Comment out the readlines and add hardcoded values below to speed up testing
                //int validHCSBundlesToGenerate = 1;
                //int invalidHCSBundlesToGenerate = 1;

                // Specify the number of HCS bundles to generate: Valid
                Log("How many Valid HCS Bundles to Generate?", LogType.Info);
                int validHCSBundlesToGenerate = int.Parse(Console.ReadLine());
                // Specify the number of HCS bundles to generate: Invalid
                Log("How many Invalid HCS Bundles to Generate?", LogType.Info);
                int invalidHCSBundlesToGenerate = int.Parse(Console.ReadLine());

                // Get path to put generated HCS Bundles in
                string dateTimeFolderName = DateTime.Now.ToString("yyyyMMddTHHmmss");
                path = $"{config["RootPath"]}\\{dateTimeFolderName}";
                Directory.CreateDirectory(path);

                // Generate the Test Bundles
                GenerateTestHCSBundles(path, validHCSBundlesToGenerate, invalidHCSBundlesToGenerate);
            }

            // Post the Bundles to Fhir Server
            PostAllBundlesInFolderToFhir(path);

        }

        //TODO: rework this into data generator section to be HCS specific
        private static List<string> GenerateTestHCSBundles(string path, int validBundlesToGenerate, int invalidBundlesToGenerate)
        {
            List<string> returnedBundles = new List<string>();

            int bundleTotalCount = 1;
            Log($"Generating {validBundlesToGenerate} valid bundles", LogType.Info);
            for (int k = 0; k < validBundlesToGenerate; k++)
            {
                Log($"Generating valid bundle: bundle_{bundleTotalCount}.json", LogType.Good);

                File.WriteAllText($"{path}\\bundle_{bundleTotalCount++}.json", dataGenerator.GenerateData(true));
            }

            Log($"Generating {invalidBundlesToGenerate} invalid bundles", LogType.Info);
            for (int i = 0; i < invalidBundlesToGenerate; i++)
            {
                Log($"Generating invalid bundle: bundle_{bundleTotalCount}.json", LogType.Bad);

                File.WriteAllText($"{path}\\bundle_{bundleTotalCount++}.json", dataGenerator.GenerateData(false));
            }


            return returnedBundles;
        }


        static async Task PostAllBundlesInFolderToFhir(string path)
        {
            string token = await authHandler.GetFhirServerToken(config, _httpClient);

            //Log(path);
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (file.EndsWith(".json"))
                {
                    await PostToFHIR(token, file);
                }
            }

        }

        static async Task PostToFHIR(string token, string file)
        {
            var body = File.ReadAllText(file);


            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{config["BaseFhirUrl"]}/$process-message"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                // WRITE RESPONSE TO FILE
                string responseBody = await response.Content.ReadAsStringAsync();
                string path = file.Substring(0, file.LastIndexOf("\\"));
                string bundleName = file.Substring(file.LastIndexOf("\\")).Replace("\\", "").Replace(".json", "");
                File.WriteAllText($"{path}\\{bundleName}_response.json", responseBody);


                if (response.IsSuccessStatusCode)
                    Log($"ProcessMessage success on {bundleName}", LogType.Good);
                else
                    Log($"ProcessMessage failed on {bundleName}", LogType.Bad);
            }

        }

    }

}


