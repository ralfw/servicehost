using System;
using System.Collections.Generic;
using Mono.Unix;
using Mono.Unix.Native;
using Nancy.Hosting.Self;
using servicehost.nancy.nonpublic;

namespace servicehost
{
    public class ServiceHost : IDisposable
    {
        public static void Run(Uri endpointUri) => Run(endpointUri, new Type[0]);
        public static void Run(Uri endpointUri, Type[] serviceTypes) {
            if (serviceTypes == null) throw new ArgumentNullException(nameof(serviceTypes));
            using (var servicehost = new ServiceHost()) {
                servicehost.Start(endpointUri, serviceTypes);
                Console.WriteLine("Service host running on {0}...", endpointUri.ToString());

                if (Is_running_on_Mono) {
                    Console.WriteLine("Ctrl-C to stop service host");
                    UnixSignal.WaitAny(UnixTerminationSignals);
                }
                else {
                    Console.WriteLine("ENTER to stop service host");
                    Console.ReadLine();
                }

                Console.WriteLine("Stopping service host");
                servicehost.Stop();
            }
        }
        
        
        
        NancyHosting nancyHost;

        public void Start(Uri endpoint) => Start(endpoint, new Type[0]);
        public void Start(Uri endpoint, IEnumerable<Type> serviceTypes) {
            var collector = new ServiceCollector();
            this.nancyHost = new NancyHosting();

            //Console.WriteLine($"Compiling services...");
            var services = collector.Collect(serviceTypes);
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
        

	    
        private static bool Is_running_on_Mono => Type.GetType("Mono.Runtime") != null;

        private static UnixSignal[] UnixTerminationSignals =>  new[] {
            new UnixSignal(Signum.SIGINT),
            new UnixSignal(Signum.SIGTERM),
            new UnixSignal(Signum.SIGQUIT),
            new UnixSignal(Signum.SIGHUP)
        };
    }
}