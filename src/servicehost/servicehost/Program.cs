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
                Console.WriteLine("Service host running on {0}...", cli.Endpoint.ToString());

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

        private static bool Is_running_on_Mono => Type.GetType("Mono.Runtime") != null;

        private static UnixSignal[] UnixTerminationSignals =>  new[] {
			new UnixSignal(Signum.SIGINT),
			new UnixSignal(Signum.SIGTERM),
			new UnixSignal(Signum.SIGQUIT),
			new UnixSignal(Signum.SIGHUP)
		};
    }
}