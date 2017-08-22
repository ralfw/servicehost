using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using servicehost.contract;

namespace demoservice
{
    public class AddRequest { 
        public int A { get; set; }
        public int B { get; set; }
    }

    public class AddResult {
        public int Sum { get; set; }
    }

    public class SimulationRequest { 
        public int[] HistoricalData { get; set; }
        public int NumberOfEvents { get; set; }
        public int NumberOfSimulations { get; set; }
    }

    public class SimulationResult { 
        public int[] Forecasts { get; set; }
    }


    [Service]
    public class SimpleService
    {
        // Usage: /add?A=1&B=2
        [EntryPoint(HttpMethods.Get, "/add")]
        public AddResult Add(int a, int b) {
            Console.WriteLine("Add({0},{1})", a, b);

            var simplemath = new Math();
            var sum = simplemath.Add(a, b); 

            return new AddResult { Sum = sum };
        }


        // Usage: /forcecast/123 + data in payload
        [EntryPoint(HttpMethods.Post, "/forecast/{id}")]
        public SimulationResult Forecast(string id, [Payload] SimulationRequest req) { 
            Console.WriteLine("Forecast requested for {0}", id);

            var forecasts = MonteCarloForecast.Simulate(req.HistoricalData, req.NumberOfEvents, req.NumberOfSimulations);

            return new SimulationResult { Forecasts = forecasts };
        }
    }
}