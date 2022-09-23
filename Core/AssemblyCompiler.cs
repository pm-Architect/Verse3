using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Verse3
{
    public static class AssemblyCompiler
    {
        #region Properties
        public static List<string> CompileLog { get; set; } = new List<string>();
        private static List<MetadataReference> references { get; set; } = new List<MetadataReference>();
        #endregion

        internal static void Init()
        {
            if (references == null || references.Count == 0)
            {
                references = new List<MetadataReference>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }
                    var name = assembly.GetName().Name + ".dll";
                    Console.WriteLine(name);
                    string loc = assembly.Location;
                    try
                    {
                        if (loc != String.Empty && File.Exists(loc))
                            references.Add(MetadataReference.CreateFromFile(loc));
                        else
                            CoreConsole.Log("Error loading assembly at :" + loc);
                    }
                    catch (Exception ex)
                    {
                        CoreConsole.Log(ex.Message + " at " + loc);
                        //throw new Exception(ex.Message);
                    }
                }
            }
        }

        public static Assembly Compile(string code)
        {
            AssemblyCompiler.Init();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview));
            foreach (var diagnostic in syntaxTree.GetDiagnostics())
            {
                CompileLog.Add(diagnostic.ToString());
            }

            if (syntaxTree.GetDiagnostics().Any(i => i.Severity == DiagnosticSeverity.Error))
            {
                CompileLog.Add("Parse SyntaxTree Error!");
                return null;
            }

            CompileLog.Add("Parse SyntaxTree Success");

            CSharpCompilation compilation = CSharpCompilation.Create("Verse3.Vanilla", new[] { syntaxTree },
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream stream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(stream);

                foreach (var diagnostic in result.Diagnostics)
                {
                    CompileLog.Add(diagnostic.ToString());
                }

                if (!result.Success)
                {
                    CompileLog.Add("Compilation error");
                    return null;
                }

                CompileLog.Add("Compilation success!");

                stream.Seek(0, SeekOrigin.Begin);

                //                var context = new CollectibleAssemblyLoadContext();
                Assembly assembly = AppDomain.CurrentDomain.Load(stream.ToArray());
                return assembly;
            }
        }

        internal static Type CompileOnly(string code)
        {
            AssemblyCompiler.Init();

            var assembly = AssemblyCompiler.Compile(code);
            if (assembly != null)
            {
                return assembly.GetExportedTypes().FirstOrDefault();
            }

            return null;
        }

        internal static IElement CreateRunClass(Type type)
        {
            try
            {
                //TODO: TODO: IMPORTANT!! Implement parameter passing for parametric construction of elements at load time
                //Use case example: License key for a paid element in a library can be passed as a parameter and checked by the constructor on load
                var instance = Activator.CreateInstance(type) as IElement;
                if (instance != null)
                {
                    return instance;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                //throw ex;
                return null;
            }
        }
    }
}
