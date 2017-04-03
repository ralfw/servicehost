using System;
using System.Collections.Generic;
using RestSharp;
using servicehost;

namespace democlient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var servicehost = new ServiceHost()) {
                servicehost.Start(new Uri("http://localhost:1234"));

                Call_math_service();
                Call_forecasting_service();

                Console.Write("Press ENTER to terminate program: ");
                Console.ReadLine();
            }
        }


        static void Call_math_service()
        {
            var rest = new RestClient("http://localhost:1234");
            var req = new RestRequest("/add", Method.GET);
            req.AddQueryParameter("A", "3");
            req.AddQueryParameter("B", "4");

            var reply = rest.Execute<AddResult>(req);
            Console.WriteLine("  Result of add service call: {0}", reply.Data.Sum);
        }


        static void Call_forecasting_service()
        {
            var rest = new RestClient("http://localhost:1234");
            var req = new RestRequest("/forecast", Method.POST); // needs to be POST for RestSharp to send JSON in payload
            req.RequestFormat = DataFormat.Json;
            var simReq = new SimulationRequest {
                HistoricalData = new[] { 1, 7, 9, 3, 4, 4, 8, 2, 5, 9, 6, 3, 2, 5, 6, 3, 1, 5, 4, 7, 8, 3, 6, 2 },
                NumberOfEvents = 3,
                NumberOfSimulations = 10
            };
            req.AddBody(simReq);

            var reply = rest.Execute<SimulationResult>(req);
            Console.WriteLine("  Result of forecast service call: {0}", string.Join(",",reply.Data.Forecasts));
        }
   }


    public class AddResult
    {
        public int Sum { get; set; }
    }


    public class SimulationRequest
    {
        public int[] HistoricalData { get; set; }
        public int NumberOfEvents { get; set; }
        public int NumberOfSimulations { get; set; }
    }

    public class SimulationResult
    {
        public List<int> Forecasts { get; set; }
        // Needs to be List<int> instead of int[] for RestSharp's deserializer :-/
    }
}
