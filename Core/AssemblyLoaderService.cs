using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verse3
{
    public class AssemblyLoaderService
    {
        //private static AssemblyCompilerService _compileService;

        //public static void Init()
        //{
        //    //_compileService = compileService;
        //}
        
        public static IEnumerable<IElement> Load(MemoryStream ms)
        {
            List<IElement> foundCommands = new List<IElement>();

            var assembly = System.Reflection.Assembly.Load(ms.ToArray());
            System.Diagnostics.Trace.WriteLine(assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                System.Diagnostics.Trace.WriteLine(type.FullName);

                if (typeof(IElement).IsAssignableFrom(type))
                {
                    System.Diagnostics.Trace.WriteLine($"Loading {type.FullName}");
                    var command = AssemblyCompilerService.CreateRunClass(type);
                    foundCommands.Add(command);
                }
            }
            return foundCommands;
        }
    }
}
