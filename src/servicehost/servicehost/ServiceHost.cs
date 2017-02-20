using System;
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

            var services = collector.Collect();
            this.nancyHost.Start(endpoint, services);
        }

        public void Stop() {
            this.nancyHost.Stop();
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}