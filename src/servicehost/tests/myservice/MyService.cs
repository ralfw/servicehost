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

        // Usage: GET /echo?text=hello-$datetime -> "hello-21.08.2017"
        [EntryPoint(HttpMethods.Get, "/echo")]
        public string Echo(string ping) {
            Console.WriteLine("MyService.Echo: {0}", ping);
            return ping.Replace("$datetime", DateTime.Now.ToString());
        }

        // Usage: GET /now -> "21.08.2017"
        [EntryPoint(HttpMethods.Get, "/now")]
        public string Now() {
            Console.WriteLine("MyService.Now");
            return DateTime.Now.ToString();
        }

        // Usage: POST /reflection/123 + { "Data": "hello $datetime" } -> { "Data": "hello 21.08.2018 - 123" }
        [EntryPoint(HttpMethods.Post, "/reflection/{id}")]
        public JsonPayload JsonDeser(string id, [Payload] JsonPayload payload) {
            Console.WriteLine("MyService.JsonDeser");
            payload.Data = payload.Data.Replace("$datetime", DateTime.Now.ToString()) + " - " + id;
            return payload;
        }
    }

    public class JsonPayload {
        public string Data;
    }
}