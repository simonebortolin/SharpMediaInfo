using GetMediaInfoGUI.Util;
using System;
using System.IO;

namespace GetMediaInfoGUI.Logging
{
    class Logger
    {
        #region Singleton
        private static Logger _instance;
        public static Logger Instance {
            get {
                if (_instance == null) {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
        #endregion

        #region Fields
        private readonly string fileName = Path.Combine(FolderUtils.GetAppDataFolder(), "error.log");

        private readonly string uniqueId = Guid.NewGuid().ToString("N");

        private LogLevel currLogLevel = LogLevel.ERROR;
        #endregion

        #region Methods
        public void SetLogLevel(LogLevel logLevel)
        {
            currLogLevel = logLevel;
        }

        public void Debug(string message)
        {
            var content = FormatMessage($"{message}", LogLevel.DEBUG);

            AppendTextToFile(content, LogLevel.DEBUG);
        }

        public void Info(string message)
        {
            var content = FormatMessage($"{message}", LogLevel.INFO);

            AppendTextToFile(content, LogLevel.INFO);
        }

        public void Warn(string message)
        {
            var content = FormatMessage($"{message}", LogLevel.WARN);

            AppendTextToFile(content, LogLevel.WARN);
        }

        public void Error(string message)
        {
            var content = FormatMessage($"{message}", LogLevel.ERROR);

            AppendTextToFile(content, LogLevel.ERROR);
        }

        public void Error(string message, Exception exception)
        {
            var content = FormatMessage($"{message}. Exception details:{Environment.NewLine}" +
                $"Message:{exception.Message}{Environment.NewLine}" +
                $"Stacktrace:{exception.StackTrace}{Environment.NewLine}" +
                $"Source:{exception.Source}{Environment.NewLine}" +
                $"Target site:{exception.TargetSite}{Environment.NewLine}", LogLevel.ERROR);

            AppendTextToFile(content, LogLevel.ERROR);
        }

        public void Error(Exception exception)
        {
            Error("An exception have been thrown during program execution.", exception);
        }
        #endregion

        #region Helpers
        private void AppendTextToFile(string content, LogLevel logLevel)
        {
            if (currLogLevel >= logLevel)
            {
                File.AppendAllText(fileName, content + Environment.NewLine);
            }
        }

        private string FormatMessage(string message, LogLevel logLevel = LogLevel.INFO)
        {
            return $"{DateTime.Now.ToString("o")} {uniqueId} [{logLevel}] {message}";
        }
        #endregion
    }
}
