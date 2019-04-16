using System.Globalization;

namespace System
{
    static class DateTimeExtensions
    {
        public static string ExecutingOfTime(this DateTime extension)
        {
            TimeSpan endExecute = DateTime.Now.Subtract(extension);
            return $"Executing of time (seconds): {endExecute.TotalSeconds.ToString("F2", CultureInfo.InvariantCulture)} seconds";
        }

        public static string ToIsoStandard(this DateTime standard)
        {
            return standard.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}