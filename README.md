# Service Host
If you want to make some functionality easily available via HTTP Service Host is here to help.
You just have to be able to conform to a very simple service contract.

Let's assume you want to publish this via HTTP:
```
class SimpleMath {
    public int Add(int a, int b) { return a + b; }
}
```
With Service Host you'd write a small wrapper class, a service:
```
using servicehost.contract;

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
```
The class is labled as a service by putting the `[Service]` attribute on it.
This attribute like the others is defined in `servicehost.contract.dll` which you need to reference from your service assembly.

Public methods you want to make accessible via HTTP as a service then need to be labled as entry points, again with an attribute.
Each entry point states the HTTP method to use and a route, i.e. `/add`. Also it needs to tell Service Host where it expects any
input data to come from. Either choose the URL query string (like above) or the HTTP payload (the default). In any case input data is
delivered as a JSON string. Output data needs also needs to be returned as a JSON string.

In the example above .NET's own JSON (de)serializer is used from the `System.Web.Extensions' assembly. It parses the incoming data into
an `AddRequest` object and nicely serializes the result object. Between clients and service the JSON contract is looking like this:
```
// input
{
  "A": 4,
  "B": 3
}

// output
{
  "Sum": 7
}
```
ddd
