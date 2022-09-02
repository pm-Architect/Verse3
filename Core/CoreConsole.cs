using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Text;
using Supabase;

namespace Core
{
    public class CoreConsole
    {
        private static readonly CoreConsole Instance = new CoreConsole();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void PushToServer(string message)
        {
            //TODO
        }

        //TODO: Debug / Events / Serial / Console Logger
        //NLog?
        // https://github.com/NLog/NLog

        private CoreConsole()
        {
            if (Client.Instance != null)
            {
                Initialize();
            }
        }

        public static async void sLoad(string url, string key)
        {
            await Client.InitializeAsync(url, key);
        }

        public static void Initialize()
        {
            var config = new LoggingConfiguration();
            
            // Targets where to log to: File and Console
            var logemail = new NLog.Targets.MailTarget("logemail");
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;

            //Supabase vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

            // Make sure you set these (or similar)
            //string url = Environment.GetEnvironmentVariable("SUPABASE_URL");
            //string key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
            string url = "https://kbtksyoszkavumwphzai.supabase.co";
            string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtidGtzeW9zemthdnVtd3BoemFpIiwicm9sZSI6ImFub24iLCJpYXQiOjE2NjIxNDMzMjYsImV4cCI6MTk3NzcxOTMyNn0.RLksej-rfUf5tcDZFgPQWhFC2r9FbVaUELLDsNvqhZk";

            sLoad(url, key);
            // That's it - forreal. Crazy right?

            // The Supabase Instance can be accessed at any time using:
            //  Supabase.Client.Instance {.Realtime|.Auth|etc.}
            // For ease of readability we'll use this:
            //var instance = Client.Instance;
        }
    }

    public class ElementConsole
    {
        public IElement Element;
        public List<string> LogBook { get; private set; } = new List<string>();
        public int Log(string message, LogMessageSeverity severity = LogMessageSeverity.Info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(severity.ToString().ToUpper());
            sb.Append("[");
            sb.Append(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("]=\"");
            sb.Append(message);
            sb.Append("\"@");
            sb.Append(Element.ElementType);
            sb.Append("{ID:");
            sb.Append(Element.ID.ToString());
            sb.Append("};");
            LogBook.Add(sb.ToString());
            return LogBook.Count;
        }
        public ElementConsole(IElement element)
        {
            Element = element;
        }
    }

    public enum LogMessageSeverity
    {
        Info,
        Warning,
        Error
    }
}
