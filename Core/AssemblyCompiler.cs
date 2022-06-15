using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Verse3
{
    public static class AssemblyCompiler
    {
        #region Properties
        public static List<string> CompileLog { get; set; }
        private static List<MetadataReference> references { get; set; }
        #endregion

        public static void Init()
        {
            if (references == null)
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
                    references.Add(MetadataReference.CreateFromFile(name));
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
        
        public static Type CompileOnly(string code)
        {
            AssemblyCompiler.Init();

            var assembly = AssemblyCompiler.Compile(code);
            if (assembly != null)
            {
                return assembly.GetExportedTypes().FirstOrDefault();
            }
            
            return null;
        }

        public static IElement CreateRunClass(Type type)
        {
            var instance = Activator.CreateInstance(type) as IElement;
            return instance;
        }
    }
}
