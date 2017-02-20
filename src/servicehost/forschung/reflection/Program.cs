using System;
using System.IO;
using System.Reflection;

using System.Linq;

namespace reflection
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var assemblyFilenames = Directory.GetFiles(".", "*.dll");
            foreach (var f in assemblyFilenames)
            {
                Console.WriteLine(f);

                var assm = Assembly.LoadFrom(f);

                var services = assm.GetTypes().Where(t => t.GetCustomAttribute<servicehost.contract.ServiceAttribute>() != null);
                foreach (var s in services)
                {
                    Console.WriteLine("  service: {0}", s.Name);

                    var methods = s.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(m => m.GetCustomAttribute < servicehost.contract.EntryPointAttribute>() != null);
                    foreach (var m in methods) {
                        Console.WriteLine("    entrypoint: {0}", m.Name);
                        var attr = (servicehost.contract.EntryPointAttribute)m.GetCustomAttribute<servicehost.contract.EntryPointAttribute>();
                        Console.WriteLine($"      {attr.HttpMethod} -- {attr.HttpRoute} -- {attr.InputSource}");

                        if (m.Name == "JsonDeser")
                        {
                            var o = Activator.CreateInstance(s);
                            var json = "{ \"Data\":\"$datetime\"}";
                            var result = (string)s.InvokeMember("JsonDeser", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, o, new[] { json });
                            Console.WriteLine("JSON: {0}",result);
                        }
                    }

                    methods = s.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                               .Where(m => m.GetCustomAttribute<servicehost.contract.SetupAttribute>() != null);
                    foreach (var m in methods)
                        Console.WriteLine("    setup: {0}", m.Name);

                    methods = s.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                               .Where(m => m.GetCustomAttribute<servicehost.contract.TeardownAttribute>() != null);
                    foreach (var m in methods)
                        Console.WriteLine("    teardown: {0}", m.Name); 
                }
            }
        }
    }
}
