using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using servicehost.nancy;

namespace servicehost
{
    public class ServiceHost : IDisposable
    {
        NancyHosting nancyHost;

        public void Start(Uri endpoint) {
            var collector = new ServiceCollector();
            this.nancyHost = new NancyHosting();

            //Console.WriteLine($"Compiling services...");
            var services = collector.Collect();
            //Log(services);
            this.nancyHost.Start(endpoint, services);
        }

        public void Stop() {
            this.nancyHost.Stop();
        }

        public void Dispose()
        {
            this.Stop();
        }


        private void Log(IEnumerable<nonpublic.ServiceInfo> services) {
            foreach (var s in services)
                Console.WriteLine($"found service entrypoint {s.EntryPointMethodname}");
        }
    }
}