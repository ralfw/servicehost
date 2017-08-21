using System;
using System.Web.Script.Serialization;
using servicehost.contract;
using System.Linq;

namespace yourservice
{
    public class AddRequest {
        public int A { get; set; }
        public int B { get; set; }
    }

    public class AddResult {
        public int Result {get;set;}
    }


    [Service]
    public class YourService
    {
        // Usage: POST /add + { "A":1, "B":2 } -> { "Result":3 }
        [EntryPoint(HttpMethods.Post, "/add")]
        public AddResult Add(AddRequest req) {
            Console.WriteLine("YourService.Add");
            var result = req.A + req.B;
            return new AddResult { Result = result };
        }


        // Usage: GET /reverse?text=abc -> "cba"
        [EntryPoint(HttpMethods.Get, "/reverse")]
        public string Reverse(string text) {
            Console.WriteLine("YourService.Reverse");
            var result = new string(text.ToCharArray().Reverse().ToArray());
            return result;
        }
    }
}