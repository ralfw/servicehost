using System;
using System.Web.Script.Serialization;
using servicehost.contract;

namespace myservice
{
    public class AddRequest {
        public int A;
        public int B;
    }

    [Service]
    public class YourService
    {
        [EntryPoint(HttpMethods.Post, "/add")]
        public string Add(string input)
        {
            Console.WriteLine("YourService.Add");
            AddRequest req = new JavaScriptSerializer().Deserialize<AddRequest>(input);
            var result = req.A + req.B;
            return "{\"result\": " + result.ToString() + "}";
        }
    }
}