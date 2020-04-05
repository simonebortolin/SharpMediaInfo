using System;
using System.IO;

namespace GetMediaInfoGUI.Logging
{
    class Logger
    {
        private static readonly string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");

        public static void Error(string message)
        {
            var content = FormatMessage($"{message}", LogLevel.ERROR);

            File.AppendAllText(fileName, content);
        }

        public static void Error(string message, Exception exception)
        {
            var content = FormatMessage($"{message}. Exception details:{Environment.NewLine}" +
                $"Message:{exception.Message}{Environment.NewLine}" +
                $"Stacktrace:{exception.StackTrace}{Environment.NewLine}" +
                $"Source:{exception.Source}{Environment.NewLine}" +
                $"Target site:{exception.TargetSite}{Environment.NewLine}", LogLevel.ERROR);

            File.AppendAllText(fileName, content);
        }

        public static void Error(Exception exception)
        {
            var content = FormatMessage($"An exception have been thrown during program execution. Exception details:{Environment.NewLine}" +
                $"Message:{exception.Message}{Environment.NewLine}" +
                $"Stacktrace:{exception.StackTrace}{Environment.NewLine}" +
                $"Source:{exception.Source}{Environment.NewLine}" +
                $"Target site:{exception.TargetSite}{Environment.NewLine}", LogLevel.ERROR);

            File.AppendAllText(fileName, content);
        }

        private static string FormatMessage(string message, LogLevel logLevel = LogLevel.INFO)
        {
            return $"{DateTime.Now.ToString("o")} [{logLevel}] {message}";
        }
    }
}
