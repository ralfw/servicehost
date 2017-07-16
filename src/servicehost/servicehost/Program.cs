using System;
using Mono.Unix;
using Mono.Unix.Native;

namespace servicehost
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var cli = new CLI(args);
            using (var servicehost = new ServiceHost())
            {
                servicehost.Start(cli.Endpoint);

                Console.WriteLine("Service host running on " + cli.Endpoint.ToString());

                if (IsRunningOnMono())
                {
                    var terminationSignals = GetUnixTerminationSignals();
                    UnixSignal.WaitAny(terminationSignals);
                }
                else
                {
                    Console.ReadLine();
                }

                Console.WriteLine("Stopping service host");
                servicehost.Stop();
            }
        }

        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private static UnixSignal[] GetUnixTerminationSignals()
        {
            return new[]
            {
              new UnixSignal(Signum.SIGINT),
              new UnixSignal(Signum.SIGTERM),
              new UnixSignal(Signum.SIGQUIT),
              new UnixSignal(Signum.SIGHUP)
            };
        }
    }
}