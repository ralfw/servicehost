using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;

namespace servicehost.nonpublic
{
    public enum HttpMethods { 
        Get,
        Post,
        Put,
        Delete
    }


    public class ServiceInfo {
        public Type ServiceType;
        public string EntryPointMethodname;
        public string SetupMethodname;
        public string TeardownMethodname;

        public HttpMethods HttpMethod;
        public string Route;

        public ServiceParameter[] Parameters;
        public Type ResultType;
    }

    public class ServiceParameter
    {
        public string Name;
        public Type Type;
        public bool IsPayload;
    }
}