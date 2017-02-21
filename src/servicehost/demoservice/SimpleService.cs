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
        [EntryPoint(HttpMethods.Get, "/add", InputSources.Querystring)]
        public string Add(string input) {
            var json = new JavaScriptSerializer();
            AddRequest req = json.Deserialize<AddRequest>(input);

            var simplemath = new Math();
            var sum = simplemath.Add(req.A, req.B);

            var result = new AddResult { Sum = sum };
            return json.Serialize(result);
        }

        [EntryPoint(HttpMethods.Post, "/forecast")]
        public string Forecast(string input) { 
            var json = new JavaScriptSerializer();
            SimulationRequest req = json.Deserialize<SimulationRequest>(input);

            var forecasts = MonteCarloForecast.Simulate(req.HistoricalData, req.NumberOfEvents, req.NumberOfSimulations);

            var result = new SimulationResult { Forecasts = forecasts };
            return json.Serialize(result);
        }
    }
}