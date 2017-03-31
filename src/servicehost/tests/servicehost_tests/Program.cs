using System;
using servicehost;

namespace servicehost_tests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var servicehost = new ServiceHost())
            {
                servicehost.Start(new Uri("http://localhost:1234"));

                Console.WriteLine("Service host running @ localhost:1234...");
                Console.WriteLine("Hit ENTER to exit");
                Console.ReadLine();
            }
        }
    }
}
