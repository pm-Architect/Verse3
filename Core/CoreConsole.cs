﻿using NLog;
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
        private static readonly StringBuilder StringLogger = new StringBuilder();
        private static readonly List<string> LogList = new List<string>();

        public static void GetEntries(int v, out string[] entries)
        {
            List<string> eList = new List<string>();
            for (int i = 0; i < v; i++)
            {
                if (LogList.Count - i - 1 >= 0)
                {
                    eList.Add(LogList[LogList.Count - i - 1]);
                }
            }
            entries = eList.ToArray();
        }
        public static void Log(string message, bool isException = false, string prefix = "", string suffix = "")
        {
            if (isException)
            {
                StringLogger.AppendLine($"ERROR: {prefix} |[ {message} ]| {suffix}");
                LogList.Add($"ERROR: {prefix} |[ {message} ]| {suffix}");
                Logger.Log(LogLevel.Error, (prefix + " |[ " + message + " ]| " + suffix).Trim());
                if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                    CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {message} ]| {suffix}"))));
            }
            else
            {
                StringLogger.AppendLine($"INFO: {prefix} |[ {message} ]| {suffix}");
                LogList.Add($"INFO: {prefix} |[ {message} ]| {suffix}");
                Logger.Log(LogLevel.Info, (prefix + " |[ " + message + " ]| " + suffix).Trim());
                if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                    CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"INFO: {prefix} |[ {message} ]| {suffix}"))));
            }
        }
        public static void Log(Exception ex, string prefix = "", string suffix = "")
        {
            StringLogger.AppendLine($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}");
            LogList.Add($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}");
            Logger.Log(LogLevel.Error, (prefix + " |[ " + ex.Message + " ]| " + suffix).Trim());
            if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}"))));
        }

        private CoreConsole()
        {
            if (Client.Instance != null)
            {
                Initialize();
            }
        }

        private static async void sLoad(string url, string key)
        {
            await Client.InitializeAsync(url, key);
        }

        public static void Initialize()
        {
            var config = new LoggingConfiguration();
            
            // Targets where to log to: File and Console
            //var logemail = new NLog.Targets.MailTarget("logemail");
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "verse3_logfile.txt",
                Layout = "${longdate} ${level} ${message} ${exception:format=tostring}"
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole") { DetectConsoleAvailable = true };
            //var logstring = new NLog.Targets.MethodCallTarget("logmethod");
            //logstring.MethodName = "LogFromNLog";
            //logstring.Parameters.Add(new NLog.Targets.MethodCallParameter("message", "${message}"));
            //logstring.Parameters.Add(new NLog.Targets.MethodCallParameter("level", "${level}"));

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logstring);
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


        public delegate void ConsoleLogEventHandler(object sender, EventArgData e);
        public static event ConsoleLogEventHandler OnLog;
    }

    //public class ElementConsole
    //{
    //    public IElement Element;
    //    public List<string> LogBook { get; private set; } = new List<string>();
    //    public int Log(string message, LogMessageSeverity severity = LogMessageSeverity.Info)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(severity.ToString().ToUpper());
    //        sb.Append("[");
    //        sb.Append(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
    //        sb.Append("]=\"");
    //        sb.Append(message);
    //        sb.Append("\"@");
    //        sb.Append(Element.ElementType);
    //        sb.Append("{ID:");
    //        sb.Append(Element.ID.ToString());
    //        sb.Append("};");
    //        LogBook.Add(sb.ToString());
    //        return LogBook.Count;
    //    }
    //    public ElementConsole(IElement element)
    //    {
    //        Element = element;
    //    }
    //}

    //public enum LogMessageSeverity
    //{
    //    Info,
    //    Warning,
    //    Error
    //}
}
