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

        // Usage: GET /deafmute -> ()
        [EntryPoint(HttpMethods.Get, "/deafmute")]
        public void Deafmute() {
            Console.WriteLine("MyService.Deafmute");
        }

        // Usage: GET /paramtypes?s=hello&i=42&b=true&d=3.14&f=3.1415&de=3.141592&i32=99&id=a1f5ca8a-9fed-498c-9cb7-a5627c49e779
        [EntryPoint(HttpMethods.Get, "/paramtypes")]
        public void ParamTypes(string s, int i, bool b, double d, float f, decimal de, Int32 i32, Guid id, DateTime da) {
            Console.WriteLine("MyService.ParamTypes");
            Console.WriteLine($"s:{s}, i:{i}, b:{b}, d:{d}, f:{f}, de:{de}, i32:{i32}, id:{id}, da:{da}");
        }
    }


    public class JsonPayload {
        public string Data { get; set; }
    }
}