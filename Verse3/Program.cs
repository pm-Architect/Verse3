using System;
using System.Windows.Forms;

namespace Verse3
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Core.Core.InitConsole();
            Application.Run(new Main_Verse3());
        }
    }
}
