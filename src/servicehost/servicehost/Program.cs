using System;

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

                Console.WriteLine("Service host running...");
                Console.ReadLine();
            }
        }
    }
}