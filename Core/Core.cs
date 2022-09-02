using Supabase;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Core
{
    public static class Core
    {
        public static event EventHandler UserLoggedIn;

        public static void InitConsole()
        {
            CoreConsole.Initialize();
        }
        public static async Task<string> Login()
        {
            string uri = await Client.Instance.Auth.SignIn(Supabase.Gotrue.Client.Provider.LinkedIn);
            HttpServer httpServer = new HttpServer();
            httpServer.Start();
            return uri;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static async void SetAuth(System.Uri uri)
        {
            Supabase.Gotrue.Session s = await Client.Instance.Auth.GetSessionFromUrl(uri);

            if (s != null)
            {
                Core.UserLoggedIn.Invoke(s, new EventArgs());

                Process currentProcess = Process.GetCurrentProcess();
                IntPtr hWnd = currentProcess.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                    //ShowWindow(hWnd, User32.SW_MAXIMIZE);
                }
            }


            //if (s != null)
            //{
            //    s.User.
            //}
            //Client.Instance.Auth.SetAuth(token);
            //return null;
        }

        public static async void Logout()
        {
            await Client.Instance.Auth.SignOut();
        }
        
        public static Supabase.Gotrue.User GetUser()
        {
            if (Client.Instance != null)
            {
                return Client.Instance.Auth.CurrentUser;
            }
            else return null;
        }

        //TODO: LOAD SETTINGS from settings.cfg json file
        //TODO:
        //LOOP LIMITING THRESHOLD : ENV_VAR


        //var cacheFileName = ".gotrue.cache";

        //async void Initialize()
        //{
        //    var options = new ClientOptions
        //    {
        //        Url = GOTRUE_URL,
        //        SessionPersistor = SessionPersistor,
        //        SessionRetriever = SessionRetriever,
        //        SessionDestroyer = SessionDestroyer
        //    };
        //    await Client.Initialize(options);
        //}

        ////...

        //internal Task<bool> SessionPersistor(Session session)
        //{
        //    try
        //    {
        //        var cacheDir = FileSystem.CacheDirectory;
        //        var path = Path.Join(cacheDir, cacheFileName);
        //        var str = JsonConvert.SerializeObject(session);

        //        using (StreamWriter file = new StreamWriter(path))
        //        {
        //            file.Write(str);
        //            file.Dispose();
        //            return Task.FromResult(true);
        //        };
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.WriteLine("Unable to write cache file.");
        //        throw err;
        //    }
        //}

    }

    public static class ComputationCore
    {

    }

    internal static class AssemblyManager
    {

    }
}
