using System;
using System.Web.Script.Serialization;
using servicehost.contract;
using System.Linq;

namespace myservice
{
    public class AddRequest {
        public int A;
        public int B;
    }

    public class ReverseRequest { 
        public string Text;
    }

    [Service]
    public class YourService
    {
        [EntryPoint(HttpMethods.Post, "/add")]
        public string Add(string input)
        {
            Console.WriteLine("YourService.Add");
            var req = new JavaScriptSerializer().Deserialize<AddRequest>(input);
            var result = req.A + req.B;
            return "{\"result\": " + result.ToString() + "}";
        }

        [EntryPoint(HttpMethods.Get, "/reverse", InputSources.Querystring)]
        public string Reverse(string input) {
            Console.WriteLine("YourService.Reverse");
            var req = new JavaScriptSerializer().Deserialize<ReverseRequest>(input);
            var result = new string(req.Text.ToCharArray().Reverse().ToArray());
            return "{\"result\": \"" + result + "\"}";
        }
    }
}