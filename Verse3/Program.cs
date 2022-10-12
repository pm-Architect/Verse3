using Core;
using NamedPipeWrapper;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Verse3
{
    internal static class Program
    {
        private static CoreInterop.InteropServer _server;
        public static Main_Verse3 main_;
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
            StartServer();

            main_ = new Main_Verse3();
            Main_Verse3.domain_ = AppDomain.CurrentDomain;
            Application.Run(main_);
        }

        //Function that creates a new InteropServer in a new thread
        public static void StartServer()
        {
            Thread serverThread = new Thread(() =>
            {
                //TODO: SECURE INTEROP THREAD
                
                _server = new CoreInterop.InteropServer("Verse3", "Verse3");
                _server.ClientMessage += OnClientMessage;
                _server.ClientConnected += OnClientConnected;
                //_server.ClientDisconnected += OnClientDisconnected;
                //_server.Error += OnError;
            });
            serverThread.Start();
        }

        private static void OnClientConnected(object sender, EventArgs e)
        {
            CoreConsole.Log("Client connected");
            NamedPipeConnection<DataStructure, DataStructure> connection = (NamedPipeConnection<DataStructure, DataStructure>)sender;
            connection.PushMessage(new DataStructure<string>("Welcome"));
        }

        private static void OnClientMessage(object sender, DataStructure e)
        {
            CoreConsole.Log($"Client sent a message: {e.ToString()}");
        }
    }
}
