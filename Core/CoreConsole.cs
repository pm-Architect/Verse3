using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Text;
using Supabase;
using Postgrest.Models;
using Postgrest.Attributes;
using Postgrest;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    public class CoreConsole
    {
        private static readonly CoreConsole Instance = new CoreConsole();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly StringBuilder StringLogger = new StringBuilder();
        private static readonly List<string> LogList = new List<string>();
        private static Table<FeedbackReport> table;

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
                SendToSB((prefix + " |[ " + message + " ]| " + suffix).Trim(), LogLevel.Error);
            }
            else
            {
                StringLogger.AppendLine($"INFO: {prefix} |[ {message} ]| {suffix}");
                LogList.Add($"INFO: {prefix} |[ {message} ]| {suffix}");
                Logger.Log(LogLevel.Info, (prefix + " |[ " + message + " ]| " + suffix).Trim());
                if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                    CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"INFO: {prefix} |[ {message} ]| {suffix}"))));
                SendToSB((prefix + " |[ " + message + " ]| " + suffix).Trim(), LogLevel.Info);
            }
        }
        public static void Log(Exception ex, string prefix = "", string suffix = "")
        {
            StringLogger.AppendLine($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}");
            LogList.Add($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}");
            Logger.Log(LogLevel.Error, (prefix + " |[ " + ex.Message + " ]| " + suffix).Trim());
            if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}"))));
            SendToSB((prefix + " |[ " + ex.Message + " ]| " + suffix).Trim(), LogLevel.Error);
        }

        public static void Log(string message, IComputable computable, bool isException = false, string prefix = "", string suffix = "")
        {
            if (isException)
            {
                StringLogger.AppendLine($"ERROR @ {computable.ID.ToString()}: {prefix} |[ {message} ]| {suffix}");
                LogList.Add($"ERROR @ {computable.ID.ToString()}: {prefix} |[ {message} ]| {suffix}");
                Logger.Log(LogLevel.Error, (computable.ID.ToString() + " : " + prefix + " |[ " + message + " ]| " + suffix).Trim());
                if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                    CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {message} ]| {suffix}"))));
                SendToSB((prefix + " |[ " + message + " ]| " + suffix).Trim(), LogLevel.Error);
            }
            else
            {
                StringLogger.AppendLine($"INFO @ {computable.ID.ToString()}: {prefix} |[ {message} ]| {suffix}");
                LogList.Add($"INFO @ {computable.ID.ToString()}: {prefix} |[ {message} ]| {suffix}");
                Logger.Log(LogLevel.Info, (computable.ID.ToString() + " : " + prefix + " |[ " + message + " ]| " + suffix).Trim());
                if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                    CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"INFO: {prefix} |[ {message} ]| {suffix}"))));
                computable.OnLog_Internal(new EventArgData(new DataStructure<string>(($"INFO: {prefix} |[ {message} ]| {suffix}"))));
                SendToSB((prefix + " |[ " + message + " ]| " + suffix).Trim(), LogLevel.Info);
            }
        }
        public static void Log(Exception ex, IComputable computable, string prefix = "", string suffix = "")
        {
            StringLogger.AppendLine($"ERROR @ {computable.ID.ToString()}: {prefix} |[ {ex.Message} ]| {suffix}");
            LogList.Add($"ERROR @ {computable.ID.ToString()}: {prefix} |[ {ex.Message} ]| {suffix}");
            Logger.Log(LogLevel.Error, (computable.ID.ToString() + " : " + prefix + " |[ " + ex.Message + " ]| " + suffix).Trim());
            if (CoreConsole.OnLog != null && CoreConsole.OnLog.GetInvocationList().Length > 0)
                CoreConsole.OnLog.Invoke(null, new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}"))));
            computable.OnLog_Internal(new EventArgData(new DataStructure<string>(($"ERROR: {prefix} |[ {ex.Message} ]| {suffix}"))));
            SendToSB((prefix + " |[ " + ex.Message + " ]| " + suffix).Trim(), LogLevel.Error);
        }

        private CoreConsole()
        {
            if (Supabase.Client.Instance != null)
            {
                Initialize();
            }
        }

        private static async void sLoad(string url, string key)
        {
            await Supabase.Client.InitializeAsync(url, key);
            //table = Postgrest.Client.Instance.Table<FeedbackReport>();
            //var task = Task.Run(() => table.Get());
            //task.Wait();
            //if (task.IsCompleted)
            //{
            //    FeedbackReport fr = task.Result.Models.FirstOrDefault();
            //    string temp = fr.Body;
            //}
        }
        
        public static void SendToSB(string message, LogLevel level)
        {
            var newMessage = new FeedbackReport { Data = message, Tags = $"{{\"log\":\"{level.Name}\"}}" };
            if (Core.GetUser() != null && table != null)
            {
                //var task = Task.Run(() => table.Get());
                //task.Wait();
                //if (task.IsCompleted)
                //{
                //    FeedbackReport fr = task.Result.Models.FirstOrDefault();
                //    string temp = fr.Body;
                //}
                //string temp = table.Get().Result.Models.First().Body;
                //table.Insert(newMessage);
                //string temp = table.BaseUrl;
            }
            else
            {
                //Table<FeedbackReport> tbl = Postgrest.Client.Instance.Table<FeedbackReport>();
                //tbl.Insert(newMessage);
            }
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
            var logmethod = new NLog.Targets.MethodCallTarget("logmethod");
            logmethod.MethodName = "SendToSB";
            logmethod.Parameters.Add(new NLog.Targets.MethodCallParameter("message", "${message}"));
            logmethod.Parameters.Add(new NLog.Targets.MethodCallParameter("level", "${level}"));

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logmethod);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;

            //Postgrest.Client.Initialize("https://kbtksyoszkavumwphzai.supabase.co");
            //table = Postgrest.Client.Instance.Table<FeedbackReport>();
            //table = new SupabaseTable<FeedbackReport>("https://kbtksyoszkavumwphzai.supabase.co", new ClientOptions());

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

    [Table("public_testing")]
    public class FeedbackReport : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("body")]
        public string Body { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FeedbackReport report &&
                    Id == report.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
