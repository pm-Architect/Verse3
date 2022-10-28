using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

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
            return Load(ms.ToArray());
        }

        public static IEnumerable<IElement> Load(byte[] bytes, AppDomain domain = null)
        {
            List<IElement> foundCommands = new List<IElement>();

            if (domain != null) domain = AppDomain.CurrentDomain;
            Assembly assembly;
            if (domain != null)
            {
                assembly = domain.Load(bytes);
            }
            else
            {
                assembly = Assembly.Load(bytes);
            }
            System.Diagnostics.Trace.WriteLine(assembly.FullName);
            CoreConsole.Log(assembly.FullName, false, "AssemblyLoader: Loading Assembly");
            Module[] modules = assembly.GetModules();
            foreach (Module module in modules)
            {
                System.Diagnostics.Trace.WriteLine(module.Name);
                CoreConsole.Log(module.Name);
                AssemblyName[] names = module.Assembly.GetReferencedAssemblies();
                if (names.Length > 0)
                {
                    foreach (AssemblyName name in names)
                    {
                        List<Assembly> loaded = AppDomain.CurrentDomain.GetAssemblies().ToList();
                        try
                        {
                            if (loaded.Any(a => a.FullName == name.FullName))
                            {
                                continue;
                            }
                            //TODO: Find and load referenced assemblies if they are not already loaded in the current domain
                            string references = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Verse3\\Libraries\\");
                            //references = Path.GetDirectoryName(assembly.);
                            //references = Path.GetDirectoryName(assembly.Location);
                            //for each file in the references folder
                            //if the file name is the same as the referenced assembly name
                            //load the file into the current domain
                            foreach (string file in Directory.GetFiles(references))
                            {
                                if (Path.GetFileNameWithoutExtension(file) == name.Name && Path.GetExtension(file) == ".dll")
                                {
                                    try
                                    {
                                        Assembly asm = Assembly.LoadFile(file);
                                        if (domain != AppDomain.CurrentDomain)
                                        {
                                            Assembly.Load(asm.GetName());
                                        }
                                        System.Diagnostics.Trace.WriteLine("Adding Reference: " + asm.FullName + " from " + file);
                                        CoreConsole.Log("Adding Reference: " + asm.FullName + " from " + file);
                                    }
                                    catch (Exception ex0)
                                    {
                                        //System.Diagnostics.Trace.WriteLine("Error Adding Reference: " + ex0.Message);
                                        CoreConsole.Log(ex0, "Error Adding Reference: ");
                                        //System.Diagnostics.Trace.WriteLine("Adding Reflection Only Reference from " + file);
                                        //Assembly asm1 = Assembly.ReflectionOnlyLoadFrom(file);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //System.Diagnostics.Trace.WriteLine("Error Adding Reference: " + ex.Message);
                            CoreConsole.Log(ex);
                        }
                        loaded = AppDomain.CurrentDomain.GetAssemblies().ToList();
                        if (loaded.Any(a => a.FullName == name.FullName))
                        {
                            System.Diagnostics.Trace.WriteLine("Added Reference: " + name.FullName);
                            CoreConsole.Log("Added Reference: " + name.FullName);
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine("Error Adding Reference: " + name.FullName);
                            CoreConsole.Log("Error Adding Reference: " + name.FullName);
                        }
                    }
                }
            }
            try
            {
                assembly = Assembly.Load(bytes);
                if (assembly.DefinedTypes != null)
                {
                    assembly.ToString();
                }
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    System.Diagnostics.Trace.WriteLine(type.FullName);
                    CoreConsole.Log(type.FullName);

                    if ((typeof(IElement).IsAssignableFrom(type)) || (type.GetInterface("IElement") != null))
                    {
                        System.Diagnostics.Trace.WriteLine($"Loading {type.FullName}");
                        CoreConsole.Log($"Loading {type.FullName}");
                        var command = AssemblyCompiler.CreateRunClass(type);
                        if (command != null) foundCommands.Add(command);
                    }
                }
            }

            catch (ReflectionTypeLoadException ex1)
            {
                //System.Diagnostics.Trace.WriteLine("Reflection Type Load Error: " + ex1.Message);
                CoreConsole.Log(ex1, "Reflection Type Load Error: ");
            }
            return foundCommands;
        }

        public static IEnumerable<IElement> Load(Assembly assembly)
        {
            List<IElement> foundCommands = new List<IElement>();
            System.Diagnostics.Trace.WriteLine(assembly.FullName);
            CoreConsole.Log(assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                System.Diagnostics.Trace.WriteLine(type.FullName);
                CoreConsole.Log(type.FullName);

                if (typeof(IElement).IsAssignableFrom(type))
                {
                    System.Diagnostics.Trace.WriteLine($"Loading {type.FullName}");
                    CoreConsole.Log($"Loading {type.FullName}");
                    var command = AssemblyCompiler.CreateRunClass(type);
                    if (command != null) foundCommands.Add(command);
                }
            }
            return foundCommands;
        }

        //public static bool LoadReferenceAssembly(Assembly assembly)
        //{

        //}
    }
}
