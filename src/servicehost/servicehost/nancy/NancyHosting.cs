using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using servicehost.nonpublic;

namespace servicehost.nancy
{
    class NancyHosting : IDisposable
    {
        NancyHost host;

        public void Start(Uri endpointAddress, IEnumerable<ServiceInfo> services) {
            var bootstrapper = new NancyBootstrapper(services);
            this.host = new NancyHost(bootstrapper, endpointAddress);
            this.host.Start();
        }

        public void Stop() {
            this.host.Stop();
        }

        public void Dispose() {
            Stop();
        }
    }
}