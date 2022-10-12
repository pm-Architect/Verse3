using Supabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
        public static int SystemCoreCount
        {
            get
            {
                return Environment.ProcessorCount / 2;
            }
        }
        private static Dictionary<string, Thread> threads = new Dictionary<string, Thread>();
        public static void Compute(IComputable computable)
        {
            try
            {
                if (threads.Count > 0)
                {
                    foreach (Thread t in threads.Values)
                    {
                        if (t != null)
                        {
                            if (t.IsAlive)
                            {
                                if (t.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    if (t.ThreadState == System.Threading.ThreadState.Aborted ||
                                        t.ThreadState == System.Threading.ThreadState.Stopped ||
                                        t.ThreadState == System.Threading.ThreadState.Unstarted)
                                    {
                                        threads.Remove(t.Name);
                                    }
                                }
                            }
                            else
                            {
                                threads.Remove(t.Name);
                            }
                        }
                    }
                }
                //TODO: collect system info
                //TODO: handle core limit reached - i.e. More threads needed than cores available
                if (SystemCoreCount >= 8)
                {
                    Thread t = new Thread(new ThreadStart(() => ComputationPipeline.ComputeComputable(computable)));
                    t.Name = "_verse_computation_thread_" + threads.Count + "_" + t.ManagedThreadId + "_" + computable.ID.ToString();
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.AboveNormal;
                    threads.Add(t.Name, t);
                    if (threads.Count > 1)
                    {
                        foreach (Thread thread in threads.Values)
                        {
                            if (thread.IsAlive)
                            {
                                if (thread.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    if (thread.ThreadState == System.Threading.ThreadState.Aborted ||
                                        thread.ThreadState == System.Threading.ThreadState.Stopped ||
                                        thread.ThreadState == System.Threading.ThreadState.Unstarted)
                                    {
                                        threads.Remove(thread.Name);
                                    }
                                }
                                else
                                {
                                    //wait for thread to complete
                                    thread.Join();
                                }
                            }
                            else
                            {
                                threads.Remove(thread.Name);
                            }
                        }
                    }
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                //throw ex;
            }
        }
    }

    public static class RenderingCore
    {
        private static Thread renderThread;
        public static void Render(IRenderable renderable)
        {
            if (renderThread != null)
            {
                if (renderThread.IsAlive)
                {
                    if (renderThread.ThreadState != System.Threading.ThreadState.Aborted &&
                        renderThread.ThreadState != System.Threading.ThreadState.Stopped &&
                        renderThread.ThreadState != System.Threading.ThreadState.Unstarted)
                    {
                        renderable.RenderExpired = true;
                        Thread renderAwait = new Thread(new ThreadStart(() =>
                        {
                            while (renderThread.ThreadState != System.Threading.ThreadState.Aborted &&
                                renderThread.ThreadState != System.Threading.ThreadState.Stopped &&
                                renderThread.ThreadState != System.Threading.ThreadState.Unstarted)
                            {
                                Thread.Sleep(1);
                            }
                        }));
                    }
                }
            }
            else
            {
                renderThread = new Thread(new ThreadStart(() => RenderPipeline.RenderRenderable(renderable)));
                renderThread.Name = "_verse_render_thread_0_" + renderThread.ManagedThreadId;
                renderThread.IsBackground = true;
                renderThread.Priority = ThreadPriority.AboveNormal;
                renderable.RenderExpired = false;
                renderThread.Start();
            }
        }
    }

    internal static class AssemblyManager
    {

    }
}
