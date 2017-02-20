using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace code_generation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var source = @"

using System.Runtime.CompilerServices;

using Nancy;
using Nancy.Extensions;

namespace MyNamespace {
    public class MyClass {
        public string Run(int input) {
            equalidator.Equalidator.AreEqual(1,1);
            return (input*3).ToString();
        }
    }

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get[""/""] = _ => ""Hello World!"";
        }
    }
}
";

            var cs = new CSharpCodeProvider();
            var p = new CompilerParameters();
            p.GenerateExecutable = false;
            p.GenerateInMemory = true;
            p.ReferencedAssemblies.Add("System.dll");
            p.ReferencedAssemblies.Add("equalidator.dll");
            p.ReferencedAssemblies.Add("nancy.dll");
            var r = cs.CompileAssemblyFromSource(p, source);

            foreach (var err in r.Errors)
                Console.WriteLine($"*** {err}");

            var t = r.CompiledAssembly.GetType("MyNamespace.MyClass");
            var o = Activator.CreateInstance(t);
            var result = t.InvokeMember("Run", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public, null, o, new object[] { 2 });
            Console.WriteLine($"result: {result}");
        }
    }


}
