using System;
using System.Collections.Generic;
using System.Linq;

namespace demoservice
{
    class MonteCarloForecast {
        public static int[] Simulate(int[] historicalData, int numberOfEvents, int numberOfSimulations) {
            var rnd = new Random();
            var forecasts = new List<int>();
            while (numberOfSimulations-- > 0) {
                var simulatedEvents = new List<int>();
                for (var i = 0; i < numberOfEvents; i++) {
                    var simulatedEvent = historicalData[rnd.Next(0,historicalData.Length-1)];
                    simulatedEvents.Add(simulatedEvent);
                }
                var total = simulatedEvents.Sum();
                forecasts.Add(total);
            }
            return forecasts.ToArray();
        }
    }
}