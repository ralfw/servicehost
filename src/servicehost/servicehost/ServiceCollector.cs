using System;
using System.Collections.Generic;
using System.Linq;
using servicehost.nonpublic;
using servicehost.contract;
using System.Reflection;
using System.IO;

namespace servicehost
{
    class ServiceCollector
    {
        public IEnumerable<ServiceInfo> Collect() => Collect(new Type[0]);
        public IEnumerable<ServiceInfo> Collect(IEnumerable<Type> serviceTypeCandidates) {
            var assemblies = Collect_assemblies().ToArray();
            //foreach (var a in assemblies) Console.WriteLine($"assembly found: {a.FullName}");
            var types = Collect_service_types(assemblies)
                        .Concat(Keep_service_types(serviceTypeCandidates));
            return Compile_services(types);
        }

        IEnumerable<Assembly> Collect_assemblies() {
            var currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"Collect_assemblies(): path={currentAssemblyPath}");
            var assemblyFilenames = Directory.GetFiles(currentAssemblyPath, "*.dll")
                                             .Concat(Directory.GetFiles(".", "*.dll"));
            foreach (var f in assemblyFilenames) {
                Console.WriteLine($"    assembly={f}");
                yield return Assembly.LoadFrom(f);
            }
        }

        
        IEnumerable<Type> Collect_service_types(IEnumerable<Assembly> assemblies) {
            return assemblies.SelectMany(assm => Keep_service_types(assm.GetTypes()));
        }
        
        IEnumerable<Type> Keep_service_types(IEnumerable<Type> types) {
            return types.Where(t => t.GetCustomAttribute<ServiceAttribute>() != null);
        }
        

        IEnumerable<ServiceInfo> Compile_services(IEnumerable<Type> types)
        {
            return types.SelectMany(Collect_service_methods)
                        .Select(Compile_service);
        }

        IEnumerable<MethodInfo> Collect_service_methods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                       .Where(m => m.GetCustomAttribute<EntryPointAttribute>() != null);
        }

        ServiceInfo Compile_service(MethodInfo method) {
            var service = new ServiceInfo();
            service.ServiceType = method.ReflectedType;
            service.EntryPointMethodname = method.Name;

            var attr = method.GetCustomAttribute<EntryPointAttribute>();
            service.Route = attr.HttpRoute;
            service.HttpMethod = MapHttpMethod(attr.HttpMethod);

            service.SetupMethodname = Get_scaffolding_method(method.ReflectedType, typeof(servicehost.contract.SetupAttribute));
            service.TeardownMethodname = Get_scaffolding_method(method.ReflectedType, typeof(servicehost.contract.TeardownAttribute));

            service.Parameters = Compile_service_parameters(method);
            service.ResultType = method.ReturnType;

            return service;
        }

        nonpublic.HttpMethods MapHttpMethod(contract.HttpMethods httpMethod)
        {
            switch (httpMethod) {
                case servicehost.contract.HttpMethods.Post: return servicehost.nonpublic.HttpMethods.Post;
                case servicehost.contract.HttpMethods.Put: return servicehost.nonpublic.HttpMethods.Put;
                case servicehost.contract.HttpMethods.Delete: return servicehost.nonpublic.HttpMethods.Delete;
                case servicehost.contract.HttpMethods.Get:
                default: return servicehost.nonpublic.HttpMethods.Get;
            }
        }


        string Get_scaffolding_method(Type serviceType, Type scaffoldingAttributteType) {
            var method = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(m => m.GetCustomAttribute(scaffoldingAttributteType) != null);
            return method != null ? method.Name : null;
        }


        ServiceParameter[] Compile_service_parameters(MethodInfo method) {
            var prms = new List<ServiceParameter>();
            foreach(var p in method.GetParameters()) {
                var sp = new ServiceParameter() {
                    Name = p.Name,
                    Type = p.ParameterType,
                    IsPayload = p.GetCustomAttributes().Any(a => a is PayloadAttribute)
                };
                prms.Add(sp);
            }
            return prms.ToArray();
        }
   }
}