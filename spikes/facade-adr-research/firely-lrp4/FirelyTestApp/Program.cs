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