using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Verse3
{
    public static class AssemblyLoader
    {
        public static IEnumerable<IElement> LoadFile(string path)
        {
            Assembly assembly = Assembly.LoadFile(path);
            var types = assembly.GetTypes();
            var elements = new List<IElement>();
            foreach (var type in types)
            {
                if (typeof(IElement).IsAssignableFrom(type))
                {
                    var element = (IElement)Activator.CreateInstance(type);
                    elements.Add(element);
                }
            }
            return elements;
        }
        public static IEnumerable<IElement> Load(MemoryStream ms)
        {
            List<IElement> foundCommands = new List<IElement>();

            Assembly assembly = Assembly.Load(ms.ToArray());
            System.Diagnostics.Trace.WriteLine(assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                System.Diagnostics.Trace.WriteLine(type.FullName);

                if (typeof(IElement).IsAssignableFrom(type))
                {
                    System.Diagnostics.Trace.WriteLine($"Loading {type.FullName}");
                    var command = AssemblyCompiler.CreateRunClass(type);
                    if (command != null) foundCommands.Add(command);
                }
            }
            return foundCommands;
        }

        public static IEnumerable<IElement> Load(Assembly assembly)
        {
            List<IElement> foundCommands = new List<IElement>();
            System.Diagnostics.Trace.WriteLine(assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                System.Diagnostics.Trace.WriteLine(type.FullName);

                if (typeof(IElement).IsAssignableFrom(type))
                {
                    System.Diagnostics.Trace.WriteLine($"Loading {type.FullName}");
                    var command = AssemblyCompiler.CreateRunClass(type);
                    if (command != null) foundCommands.Add(command);
                }
            }
            return foundCommands;
        }
    }
}
