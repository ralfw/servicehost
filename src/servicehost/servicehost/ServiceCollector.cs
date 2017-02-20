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
        public IEnumerable<ServiceInfo> Collect()
        {
            var assemblies = Collect_assemblies().ToArray();
            var types = Collect_types(assemblies).ToArray();
            return Compile_services(types);
        }

        IEnumerable<Assembly> Collect_assemblies()
        {
            var assemblyFilenames = Directory.GetFiles(".", "*.dll");
            foreach (var f in assemblyFilenames)
                yield return Assembly.LoadFrom(f);
        }

        IEnumerable<Type> Collect_types(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(assm => assm.GetTypes()
                                                     .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null));
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

        ServiceInfo Compile_service(MethodInfo method)
        {
            var service = new ServiceInfo();
            service.ServiceType = method.ReflectedType;
            service.EntryPointMethodname = method.Name;

            var attr = method.GetCustomAttribute<EntryPointAttribute>();
            service.Route = attr.HttpRoute;
            service.HttpMethod = MapHttpMethod(attr.HttpMethod);
            service.InputSource = MapInputSource(attr.InputSource);

            service.SetupMethodname = Get_scaffolding_method(method.ReflectedType, typeof(servicehost.contract.SetupAttribute));
            service.TeardownMethodname = Get_scaffolding_method(method.ReflectedType, typeof(servicehost.contract.TeardownAttribute));

            return service;
        }

        servicehost.nonpublic.HttpMethods MapHttpMethod(servicehost.contract.HttpMethods httpMethod)
        {
            switch (httpMethod) {
                case servicehost.contract.HttpMethods.Post: return servicehost.nonpublic.HttpMethods.Post;
                case servicehost.contract.HttpMethods.Put: return servicehost.nonpublic.HttpMethods.Put;
                case servicehost.contract.HttpMethods.Delete: return servicehost.nonpublic.HttpMethods.Delete;
                case servicehost.contract.HttpMethods.Get: 
                default: return servicehost.nonpublic.HttpMethods.Get;
            }
        }

        servicehost.nonpublic.InputSources MapInputSource(servicehost.contract.InputSources inputSource)
        {
            switch (inputSource) {
                case servicehost.contract.InputSources.Payload: return servicehost.nonpublic.InputSources.Payload;
                case servicehost.contract.InputSources.Querystring:
                default: return servicehost.nonpublic.InputSources.Querystring;
            }
        }

        string Get_scaffolding_method(Type serviceType, Type scaffoldingAttributteType) { 
            var method = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(m => m.GetCustomAttribute(scaffoldingAttributteType) != null);
            return method != null ? method.Name : null;
        }
   }
}