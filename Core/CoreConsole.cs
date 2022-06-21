using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CoreConsole
    {
        //TODO: Debug / Events / Serial / Console Logger
        //NLog?
        // https://github.com/NLog/NLog
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
