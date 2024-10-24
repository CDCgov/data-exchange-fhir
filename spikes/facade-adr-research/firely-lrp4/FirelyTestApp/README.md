# Testing Firely SDK, Local Run

## Docs Followed:
https://docs.fire.ly/projects/Firely-NET-SDK/en/latest/


## Steps - Console App:
```
$ dotnet --list-sdks
8.0.306 [C:\Program Files\dotnet\sdk]
```

```
$ dotnet new console -n FirelyTestApp 
$ cd FirelyTestApp
```

```
$ dotnet add package Hl7.Fhir.R4
$ dotnet add package Newtonsoft.Json
```

Program.cs
```C#
using Hl7.Fhir.Model;
using Newtonsoft.Json;

// // See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

namespace FirelyTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello FirelyTestApp!");

            var patient = new Patient();
            Console.WriteLine(patient);

            patient.Id = "TestID";

            var patientJson = JsonConvert.SerializeObject(patient);
            Console.WriteLine(patientJson);

        }// .Main

    }// .Program

}// .namespace
```

--- 

## Steps - Web App: TODO


