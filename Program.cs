using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using OracleConfig.Entities.Exceptions;
using OracleConfig.Services;

namespace OracleConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include,
                //Converters = { new JavaScriptDateTimeConverter() },
                DateFormatString = "yyyy-MM-ddTHH:mm:ss"
            };

            try
            {
                DateTime startDateTime = DateTime.Now;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Start command generator.");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(startDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                ProcessProg(args[0]);

                TimeSpan endExecute = DateTime.Now.Subtract(startDateTime);
                Console.WriteLine($"Executing of time (seconds): {endExecute.TotalSeconds.ToString("F2", CultureInfo.InvariantCulture)} seconds");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Command generator completed.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (DomainException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"DomainException error:\r\n{e.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error:\r\n{e.Message}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void ProcessProg(string client)
        {
            IClientJsonService clientJson = new ClientOracleService(client);
            clientJson.CreateConfig();
        }
    }
}
