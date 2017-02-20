using System;
using Newtonsoft.Json;
using servicehost.contract;

namespace myservice
{
    [Service]
    public class MyService
    {
        [Setup]
        public void Arrange() {
            Console.WriteLine("MyService.Arrange");
        }

        [Teardown]
        public void Cleanup() {
            Console.WriteLine("MyService.Cleanup");
        }

        [EntryPoint(HttpMethods.Get, "/echo", InputSources.Querystring)]
        public string Echo(string input) {
            Console.WriteLine("MyService.Echo");
            return input.Replace("$datetime", DateTime.Now.ToString());
        }

        [EntryPoint(HttpMethods.Get, "/reflection")]
        public string JsonDeser(string input)
        {
            Console.WriteLine("MyService.JsonDeser");

            JsonConvert.DeserializeObject<JsonPayload>(input);

            return input.Replace("$datetime", DateTime.Now.ToString());
        }
    }

    public class JsonPayload {
        public string Data;
    }
}