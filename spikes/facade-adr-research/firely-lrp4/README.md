# Testing Firely SDK, Local Run

# Summary
1. FirelyTestApp - CLI App
2. FirelyApiApp - Api (web) App - TODO

## Firely Docs Reference

[Firely-NET-SDK](https://docs.fire.ly/projects/Firely-NET-SDK/en/latest/)

![Firely-NET-SDK](firely-web-01.PNG)

# 1. FirelyTestApp - CLI App

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

# 2. FirelyApiApp - Api (web) App - TODO

Open API (Swagger): http://localhost:5215/swagger/index.html

```
$ dotnet new webapi -n FirelyApiApp
```

```
$ dotnet add package Hl7.Fhir.R4
```





