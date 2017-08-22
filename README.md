# Service Host
If you want to make some functionality coded in a .NET language easily available via HTTP, Service Host is here to help.

## Defining a Service
Let's assume you want to publish this via HTTP:

```
class SimpleMath {
    public int Add(int a, int b) { return a + b; }
}
```

With Service Host you just sprinkle two attributes on this class (or a tiny wrapper) - and that's it:

```
using servicehost.contract;

[Service]
public class SimpleService
{
    [EntryPoint(HttpMethods.Get, "/add"]
    public int Add(int a, int b) { return a+b; }
}
```

The class is labeled as a service by putting the `[Service]` attribute on it.

This attribute like the others is defined in `servicehost.contract.dll` which you need to reference from your service assembly.

Public methods you want to make accessible via HTTP as a service then need to be labeled as entry points, again with an attribute.

Each entry point states the HTTP method to use (e.g. `GET`) and a route, e.g. `/add`.

The values for the parameters are then taken from either the URL route or the query string of the URL or the HTTP payload.

In this case you'd call the service like this: `acme.com/add?a=41&b=1`.

## Hosting a Service
To create a service it's easiest if you add Service Host to a *library project* - say `demoservice.dll` - using NuGet. You find the package [here](https://www.nuget.org/packages/servicehost/).

A couple of assemblies will then be included in your project. Most notable of them `servicehost.contract.dll` and `servicehost.exe`.

When you compile your project all assemblies end up in the same output directory. Go there and start Service Host like this:

```
mono servicehost.exe http://localhost:8080
```

(Whether you need to use `mono` or not depends on what your platform is. On Windows you'll likely leave it out. But you need it on Linux or Mac OSX.)

Service Host scans all `.dll`-assemblies in the directory and collects from them the service functions defined like above. They get published at the endpoints and your services are ready to go.


### Testing a Service
To call the service you can use curl:

```
$ curl http://localhost:1234/add?A=10&B=4
14
$
```

(Please note: Maybe you have to escape the `&` with a `\` or some other character.)

Or you use a tool like [Postman](https://www.getpostman.com):

---

So much for a first impression of Service Host. If you want to learn more, check out the [reference](doc/reference.md) page.