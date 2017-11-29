using System;
using Nancy.Hosting.Self;
using servicehost.nancy;

namespace servicehost
{
    class CLI {
        public Uri Endpoint { get; }

        public CLI(string[] args) {
            if (args.Length < 1)
            {
                var envEndpoint = Environment.GetEnvironmentVariable("SERVICEHOST_ENDPOINT");
                if (envEndpoint == null) throw new ApplicationException("Missing endpoint address! Usage example: servicehost.exe http://localhost:1234");
                
                this.Endpoint = new Uri(envEndpoint);
            }
            else
                this.Endpoint = new Uri(args[0]);
        }
    }
}