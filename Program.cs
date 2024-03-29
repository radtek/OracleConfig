﻿using System;
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
                Console.WriteLine(startDateTime.ToIsoStandard());

                if (args.Length <= 0)
                {
                    throw new DomainException("Argument undefined.");
                }

                ProcessProg(args[0]);

                //ProcessProg("client");

                Console.WriteLine(startDateTime.ExecutingOfTime());

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
            CommandService cli = new CommandService(new OracleCommandService(client));
            cli.CreateConfig();
        }
    }
}
