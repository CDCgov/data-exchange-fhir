using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirTestHCSBulkLoader.Config
{
    internal class LogConfig
    {
        const string LogPrefix = "FHIR.HCS.BulkLoader";

        public enum LogType
        {
            Info = 0,
            Good = 1,
            Warn = 2,
            Bad = 3
        }

        public static void Log(string output, LogType type = LogType.Info)
        {
            switch (type)
            {
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.White; break;
                case LogType.Good:
                    Console.ForegroundColor = ConsoleColor.Green; break;
                case LogType.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogType.Bad:
                    Console.ForegroundColor = ConsoleColor.Red; break;
            }

            Console.WriteLine($"{LogPrefix} {DateTime.Now.ToLongTimeString()}: {output}");
        }
    }
}
