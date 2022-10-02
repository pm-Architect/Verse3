using System;
#if NET48
using Rhino;
#endif
using Core;

namespace CoreRhinoInteropExtension
{
    public class CoreRhinoInteropHelper
    {
        public CoreRhinoInteropHelper()
        {
#if NET48
            RhinoApp.WriteLine("Hello from Rhino!");
            System.Media.SystemSounds.Beep.Play();
#endif
        }

        public string Info
        {
            get
            {
#if NET48
                return "Rhino";
#else
                return "Core";
#endif
            }
        }
    }
}
