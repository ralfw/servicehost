using System;

namespace servicehost.contract
{
    public enum HttpMethods { 
        Get,
        Post,
        Put,
        Delete
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class EntryPointAttribute : Attribute {
        public HttpMethods HttpMethod { get; }
        public string HttpRoute { get; }

        public EntryPointAttribute(HttpMethods httpMethod, string httpRoute)
        {
            HttpMethod = httpMethod;
            HttpRoute = httpRoute;
        }
    }
}