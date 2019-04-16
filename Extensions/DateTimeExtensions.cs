using System;
using System.Globalization;

namespace OracleConfig.Extensions
{
    static class DateTimeExtensions
    {
        public static string ExecutingOfTime(this DateTime extension)
        {
            TimeSpan endExecute = DateTime.Now.Subtract(extension);
            return $"Executing of time (seconds): {endExecute.TotalSeconds.ToString("F2", CultureInfo.InvariantCulture)} seconds";
        }
    }
}