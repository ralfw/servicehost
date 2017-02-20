using System;
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

    [Service]
    public class SimpleService
    {
        [EntryPoint(HttpMethods.Get, "/add", InputSources.Querystring)]
        public string Add(string input) {
            var json = new JavaScriptSerializer();
            AddRequest req = json.Deserialize<AddRequest>(input);

            var simplemath = new SimpleMath();
            var sum = simplemath.Add(req.A, req.B);

            var result = new AddResult { Sum = sum };
            return json.Serialize(result);
        }
    }


    class SimpleMath {
        public int Add(int a, int b) { return a + b; }
    }
}
