using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;


namespace CDC.DEX.FHIR
{
    internal enum LogEntryType
    {
        Info = 0,
        Status = 1,
        Warning = 2,
        Exception = 3,
        Title = 4,
        Heading = 5
    }

    internal enum RunType
    {
        ConnectionOnly = 0,
        UnloadOnly = 1,
        UnloadAndLoad = 2,
        LoadOnly = 3
    }

    internal class Program
    {
        // constants
        const string _app_name = "FHIR.Server.IG.Loader";

        // run-time variables (TODO: convert to run-time arguments)       
        private static readonly RunType _currentRun = RunType.LoadOnly;      // WARNING: unloads will DELETE all fhir metadata resources
        private static readonly bool _logVerbose = true;                    // verbose logging for advanced diagnostics
        private static readonly bool _fixDiv = true;                        // fix div html issue within metadata resource before loading
        private static readonly bool _fixTitle = true;                      // fix url query issue by setting title to url without url prefix

        // root folder containing FHIR metadata files
        const string _starting_path = @"C:\pkg-fhir";

        // csv, only load these fhir metadata resource types, case-sensitive
        const string _valid_types = "CapabilityStatement,StructureDefinition,ValueSet";

        // csv, ignore these folder/file names when iterating dir tree
        const string _invalid_folders = "example,openapi,other,xml,us.cdc.phinvads,us.nlm.vsac";
        const string _invalid_files = ".schema.json,example.json";

        // file load status values
        const string _load_status_loaded = "Loaded";
        const string _load_status_invalid = "Invalid";
        const string _load_status_duplicate = "Duplicate";
        const string _load_status_failed = "Failed";

        // search variables for batch unloading
        const string _batch_age_out = "1999-01-01"; // age out date for unloading batches
        const int _batch_size = 25; // size of batch when unloading metadata (between 1 and 100)


        // make sure bearer token below is active
        const string _fhir_api = @"https://apidevfhir.cdc.gov";
        const string _bearer_token = @"";


        private static readonly HttpClient _httpClient = new();        

        // file result status lists 
        private static readonly Dictionary<string, List<string>> _loadStatus = new();
        

        // fhir metadata loader starts here
        static void Main(string[] args)
        {
            try
            {
                InitializeLogging(); // uses diagnostic trace listeners
                Log("BEGIN", LogEntryType.Heading);
                InitializeLoader(); // setup fhir rest api client - throws exception on invalid connection

                if (_currentRun == RunType.UnloadOnly || _currentRun == RunType.UnloadAndLoad)
                {
                    Log("UNLOAD", LogEntryType.Heading);
                    UnloadMetadata(); // unload existing fhir metadata resources from fhir server
                }
                if (_currentRun == RunType.LoadOnly || _currentRun == RunType.UnloadAndLoad)
                {
                    Log("LOAD", LogEntryType.Heading);
                    int fileCount = 0, currentCount = 0;
                    GetFileCount(_starting_path, ref fileCount);

                    Log($"[Path]\t{_starting_path}", LogEntryType.Info);
                    Log($"[Files]\t{fileCount}", LogEntryType.Status);

                    LoadMetadata(_starting_path, fileCount, ref currentCount);  // load new fhir metadata resources (pkg files) to fhir server
                    LogLoadStatus(); // log results
                }
            }
            catch (Exception e)
            {
                Log("EXCEPTION", LogEntryType.Heading);
                Log(e.Message, LogEntryType.Exception);
            }
            finally
            {
                Log("END", LogEntryType.Heading);
                Console.WriteLine("Press any key to exit");
                Console.Read();          
            }

        }

        static void InitializeLoader()
        {
            // initialize fhir rest api client
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _bearer_token); // make sure bearer token is active

            TestConnection();  // throws exception if fhir server not reachable or not authorized

            // intialize load processing status lists
            _loadStatus.Add(_load_status_loaded, new());
            _loadStatus.Add(_load_status_invalid, new());
            _loadStatus.Add(_load_status_duplicate, new());
            _loadStatus.Add(_load_status_failed, new());

            // display counts by resource type
            foreach (var resourceType in _valid_types.Split(','))
            {
                Log($"{resourceType}={GetResourceCount(resourceType)}", LogEntryType.Status);
            }
        }

        static void TestConnection()
        {
            // fhir resource  path
            var path = $"{_fhir_api}/Patient";

            // check connection
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            try
            {
                var response = _httpClient.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    Log("Connection to FHIR server succeeded", LogEntryType.Info);
                }
                else
                {
                    throw new ApplicationException(response.StatusCode.ToString());
                }
            }
            catch (Exception e) 
            {
                string message = $"Connection to FHIR server failed [{e.Message}]";
                throw new ApplicationException(message);
            }
        }


        // unload all fhir metadata (structure defs, capabilities, value sets, etc.)
        static void UnloadMetadata()
        {
            string[] resourceTypes = _valid_types.Split(',');
            foreach (var resourceType in resourceTypes)
            {
                int count = 0;
                if (ResourcesExist(resourceType)) // remove existing resources
                {
                    UnloadResources(resourceType, ref count);
                }
            }
        }

        // check if resources exist for metadata resource type
        static bool ResourcesExist(string resourceType)
        {
            bool resourcesExist = (GetResourceCount(resourceType) > 0);
            return resourcesExist;
        }

        static int GetResourceCount(string resourceType)
        {
            int resourceCount = 0;

            // get summary count api
            var path = $"{_fhir_api}/{resourceType}?_lastUpdated=gt{_batch_age_out}&_summary=Count";

            // get count of resources by resource type to see if more exist
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            var response = _httpClient.Send(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                string responseString = responseContent ?? string.Empty;
                if (responseString != string.Empty)
                {
                    // parse json response for total resources remaining for resource type
                    try
                    {
                        JsonNode json = JsonNode.Parse(responseString);
                        int total = int.Parse(json["total"].ToString());
                        resourceCount = total;                        
                    }
                    catch (NullReferenceException)
                    {
                        resourceCount = 0;
                    }
                }
            }
            return resourceCount;
        }

        // unload fhir metadata resources for this resource type
        static void UnloadResources(string resourceType, ref int count)
        {
            // delete api (in batches)
            var path = $"{resourceType}{_fhir_api}/?_lastUpdated=gt{_batch_age_out}&_count={_batch_size}&hardDelete=true";

            // delete resources (in batches)
            var request = new HttpRequestMessage(HttpMethod.Delete, path);
            var response = _httpClient.Send(request);
            if (response.IsSuccessStatusCode)
            {
                count += _batch_size;
                // check if more need to be deleted
                if (ResourcesExist(resourceType))
                {
                    UnloadResources(resourceType, ref count); // continue unloading
                }
            }
            else
            {
                Log($"{resourceType}(s) not unloaded: {response.StatusCode}", LogEntryType.Status);
            }
        }

        static void GetFileCount(string path, ref int fileCount)
        {
            if (ValidFolder(path))
            {
                // check each file in current folder
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (ValidFile(file, out string resourceType))  // check if valid file name and type
                        fileCount++;
                }
            }

            // keep loading as you traverse the directory tree
            foreach (var folder in Directory.GetDirectories(path))
            {
                GetFileCount(folder, ref fileCount);
            }
        }

        // load fhir metadata into fhir server
        static void LoadMetadata(string path, int fileCount, ref int currentCount)
        {
            if (ValidFolder(path))
            {
                Log(path, LogEntryType.Info);

                // check each file in current folder, if valid then send to fhir server
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    string status;

                    if (ValidFile(file, out string resourceType))  // check if valid file name and type
                    {
                        if (ValidResource(file, out string title, out string version)) // check if valid fhir resource in file
                        {
                            if (ResourceExists(resourceType, title, version)) // check if already loaded
                            {
                                status = _load_status_duplicate;
                            }
                            else // valid file, valid resource, doesn't already exist, so load it
                            {
                                SendFile(file, resourceType, out status); // send to FHIR server, status will be loaded or failed
                            }
                        }
                        else
                        {
                            status = _load_status_invalid;
                        }

                        // store load status for this file
                        currentCount++;
                        _loadStatus[status].Add(file);

                        if (_logVerbose)
                        {
                            decimal pct = ((decimal)currentCount / (decimal)fileCount) * 100;
                            Log($"{pct:#00}% [{status}] {Path.GetFileName(file)}", LogEntryType.Status);
                        }
                    }
                }
            }

            // keep loading as you traverse the directory tree
            foreach (var folder in Directory.GetDirectories(path))
            {
                LoadMetadata(folder, fileCount, ref currentCount);
            }
        }

        // check if folder is valid
        static bool ValidFolder(string path)
        {
            bool validFolder = true; // assumes to be valid

            string[] invalidFolders = _invalid_folders.Split(','); // folder names to ignore
            foreach (var invalidFolder in invalidFolders)
            {
                if (path.EndsWith($"\\{invalidFolder}", StringComparison.OrdinalIgnoreCase))
                {
                    validFolder = false; // invalid
                    break;
                }
            }

            return validFolder;
        }

        // check if file name and type is valid
        static bool ValidFile(string file, out string resourceType)
        {
            bool validFile = true; // assumes to be valid
            resourceType = string.Empty;

            var name = Path.GetFileName(file);
            string[] invalidFiles = _invalid_files.Split(','); // file names to ignore

            // check if invalid file name
            foreach (var invalidFile in invalidFiles)
            {
                if (name.EndsWith($"{invalidFile}", StringComparison.OrdinalIgnoreCase))
                {
                    validFile = false; // invalid
                    break;
                }
            }

            if (validFile) // file name was valid, now check type
            {
                validFile = false;  // now assumed to be invalid

                string[] validTypes = _valid_types.Split(','); // list of valid fhir metadata resource types

                // check if valid resource type
                foreach (var validType in validTypes)
                {
                    if (name.StartsWith(validType, StringComparison.OrdinalIgnoreCase))
                    {
                        validFile = true; // match found, so valid
                        resourceType = validType;
                        break;
                    }
                }
            }

            return validFile;
        }

        // check if fhir resource in file is valid
        static bool ValidResource(string file, out string title, out string version)
        {
            bool validResource = false; // assumed to be invalid
            string url = string.Empty;
            title = string.Empty;
            version = string.Empty;

            // valid fhir resources contain a fhir url reference
            var fileContents = File.ReadAllText(file);

            try
            {
                JsonNode json = JsonNode.Parse(fileContents);
                url = json["url"].ToString();
                title = url[7..];
                version = json["version"].ToString();

                validResource = true; // url, name & version found in fhir resource json
            }
            catch (NullReferenceException)
            {
                validResource = (url != string.Empty);  // still valid if just url was found
            }

            return validResource;
        }

        // check if fhir resource already exists on fhir server
        static bool ResourceExists(string resourceType, string title, string version)
        {
            bool resourceExists = false;

            if (title != string.Empty)
            {
                // fhir resource search path
                var path = $"{_fhir_api}/{resourceType}?title:exact={title}"; // get resource by title (WAF blocks url parameter)
                if (version != string.Empty) path += $"&version={version}"; // and by version

                // check for existing resource by url and optional version
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                
                var response = _httpClient.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    string responseString = responseContent ?? string.Empty;
                    if (responseString != string.Empty)
                    {
                        // check if resource was found
                        JsonNode? data = JsonSerializer.Deserialize<JsonNode>(responseString);
                        if (data != null && data["entry"] != null)
                        {
                            resourceExists = true;
                        }
                    }
                }
            }

            return resourceExists;
        }

        // send fhir metadata file contents to fhir server
        static void SendFile(string file, string resourceType, out string status)
        {
            var path = $"{_fhir_api}/{resourceType}"; // fhir server api path

            var body = File.ReadAllText(file);
            if (_fixDiv) body = FixDiv(body);
            if (_fixTitle) body = FixTitle(body);

            // send to fhir server rest api endpoint
            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };                    

            // set status
            var response = _httpClient.Send(request);
            status = (response.IsSuccessStatusCode) ? _load_status_loaded : _load_status_failed;
        }

        // fix invalid div section in fhir metadata file contents
        static string FixDiv(string content)
        {
            Regex regex = new(@"<div(.*?)</div>"); // find div section
            MatchCollection matches = regex.Matches(content);

            Match? match = null;
            switch (matches.Count)
            {
                case 0:
                    return content;
                case 1:
                    match = matches[0];
                    content = content.Replace(match.Value, "<div>empty</div>");
                    return content;
                default:
                    for (var i = matches.Count - 1; i > -0; i--)
                    {
                        match = matches[i];
                        content = content.Replace(match.Value, "<div>empty</div>");
                    }
                    return content;
            }
        }

        static string FixTitle(string content)
        {
            try
            {
                JsonNode json = JsonNode.Parse(content);
                string url = json["url"].ToString();
                string title = url[7..];
                json["title"] = title;
                content = json.ToJsonString();
            }
            catch (NullReferenceException)
            { // nothing 
            }
            return content;
        }

        // write load status results to console
        static void LogLoadStatus()
        {
            Log("SUMMARY", LogEntryType.Heading);
            foreach (string status in _loadStatus.Keys)
            {
                Log($"[{status}] \t{_loadStatus[status].Count}", LogEntryType.Status);
            }
        }

        // initialize logging
        static void InitializeLogging()
        {
            try
            {
                System.Diagnostics.Trace.Listeners.Clear();

                var path = @$"{_starting_path}\{_app_name}_{DateTime.Now.ToFileTimeUtc()}.log";
                TextWriterTraceListener file = new(path)
                {
                    Name = _app_name,
                    TraceOutputOptions = TraceOptions.DateTime
                };

                ConsoleTraceListener console = new(false)
                {
                    TraceOutputOptions = TraceOptions.DateTime
                };

                System.Diagnostics.Trace.Listeners.Add(file);
                System.Diagnostics.Trace.Listeners.Add(console);
                System.Diagnostics.Trace.AutoFlush = true;

                Log(_app_name, LogEntryType.Title);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to initialize logging");
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;   
            }
        }

        // write output to console
        static void Log(string output, LogEntryType type = LogEntryType.Title)
        {
            // write to trace
            switch (type)
            {
                case LogEntryType.Title:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Trace.WriteLine($"{output}");
                    break;

                case LogEntryType.Heading:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Trace.WriteLine($"{output} {new string('=', 100 - output.Length)}");
                    break;

                case LogEntryType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Trace.WriteLine($"\t{type.ToString().ToLower()}\t{output}");
                    break;

                case LogEntryType.Status:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Trace.WriteLine($"\t{type.ToString().ToLower()}\t{output}");
                    break;

                case LogEntryType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Trace.WriteLine($"\t{type.ToString().ToLower()}\t{output}");
                    break;

                case LogEntryType.Exception:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.WriteLine($"\t{type.ToString().ToLower()}\t{output}");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}