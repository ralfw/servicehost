using System;
using System.Reflection;

namespace servicehost.nonpublic.nancy
{
    class ServiceAdapter {
        readonly Type type;
        readonly Object instance;
        readonly string entrypointMethodname;
        readonly string setupMethodname;
        readonly string teardownMethodname;

        public ServiceAdapter(Type serviceType, 
                       string entrypointMethodname, 
                       string setupMethodname, 
                       string teardownMethodname) {
            this.teardownMethodname = teardownMethodname;
            this.setupMethodname = setupMethodname;
            this.entrypointMethodname = entrypointMethodname;
            this.type = serviceType;
            this.instance = Activator.CreateInstance(serviceType);
        }

        public object Execute(object[] input) {
            Invoke_optionally(this.setupMethodname);
            var output = Invoke(this.entrypointMethodname, input);
            Invoke_optionally(this.teardownMethodname);
            return output;
        }

        void Invoke_optionally(string methodname) {
            if (!string.IsNullOrEmpty(methodname))
                Invoke(methodname, null);
        }

        object Invoke(string methodname, object[] parameters) {
            if (!string.IsNullOrEmpty(methodname))
                return this.type.InvokeMember(
                            methodname,
                            BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null,
                            this.instance, parameters);
            else
                throw new InvalidOperationException($"Cannot find service method '{methodname}' on service '{this.type.Name}'!");
        }
    }
}