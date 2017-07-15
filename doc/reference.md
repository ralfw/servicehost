# Service Host Reference

## Service Definition
### [Service] Attribute
To advertize a class as a service for publication by Service Host you need to annotate it with the `[Service]` attribute and make it public:

```
using servicehost.contract;
...
[Service]
public class SimpleService {
...
}
```

This attribute like all the others is define in `servicehost.contract.dll`.

Upon start Service Host will check all assemblies with the extension `.dll` in the current directory for classes with this attribute and make their entrypoints accessible via HTTP.

### [EntryPoint] Attribute
The actual service to be published is a function labled with the `[EntryPoint]` attribute. There can be any number of service functions in a service class.

```
using servicehost.contract;
...
[Service]
public class SimpleService {
  [EntyPoint(HttpMethods.Get, "/add", InputSources.Querystring)]
  public string Add(string input) {
    ...
  }
}
```

The `[EntryPoint]` attribute has three parameters:

* First you have to specify the HTTP method to use by clients to reach the entrypoint. Chosse `HttpMethods.Get` or `HttpMethods.Post` or `HttpMethods.Put` or `HttpMethods.Delete`.
* Second you have to state the URL route specific to the entrypoint. Upon calling the service it has to be appended to the Service Host URI, e.g. `http://localhost:1234/add`. The routes of all entrypoints per HTTP method on all service classes have to be distinct.
* Third you need to specify where the input data should be read from. `InputSources.Payload` means it's taken from the HTTP payload (requiring a Content-Type of "application/json"). But for simple parameters to the entrypoint you can alternatively use the URL querystring (`InputSources.Querystring`). Each querystring name/value pair will become a separate entry in the input JSON, e.g.

```
http://localhost:1234/add?A=3&B=4

// will deliver this input to the entrypoint

{
  "A": "3",
  "B": "4"
}
```

Input to the entrypoint as well as its value (output) are JSON strings. Hence the signature of an entrypoint function must be `string f(string input)`.

If an entrypoint does not expect any input data, use `InputSources.Querystring`.

### [Setup] Attribute
If you want something to happen _before_ an entrypoint is called by Service Host you can provide a public method annotated with the `[Setup]` attribute. It will be run right before the entry point method.

```
using servicehost.contract;
...
[Service]
public class SimpleService {
  [Setup]
  public void Prepare() {
    ...
  }
  ...
}
```

Currently service classes are instanciated for every HTTP call. No state can be retained between calls. But any state set up by a `[Setup]` method of course is present for use by the entrypoint.

There should only be one `[Setup]` method in a service class. Service Host will work with the first it finds via reflection.

### [Teardown] Attribute
If you want something to happen _after_ an entrypoint got called by Service Host you can provide a public method annotated with the `[Teardown]` attribute. It will be run right after the entry point method.

```
using servicehost.contract;
...
[Service]
public class SimpleService {
  ...
  [Teardown]
  public void Cleanup() {
    ...
  }
}
```

Currently service classes are instanciated for every HTTP call. No state can be retained between calls. But any state set up by a `[Setup]` method of course is still present.

There should only be one `[Teardown]` method in a service class. Service Host will work with the first it finds via reflection.

### Exception Handling
Any exceptions during service execution should be handled by the service itself. Design the output JSON data structures so that they are able to carry success as well as failure data.

However, if an exception slips through to Service Host it will be caught. It will be flagged to the client with HTTP status code 500 (internal server error). Also the exception's message will be passed back to the client in the payload of the response.

## Service Publication
### Command Line Usage
You can focus on coding just your service assembly and let the Service Host command line application do the rest. Just start Service Host like this:

```
$ mono servicehost.exe http://localhost:1234
```

The application will search for service classes in all the assemblies in the current directory. Any endpoints will then be published at then URI passed as the first argument on the command line. Choose the URI to your needs.

### Library Usage
If you want some more control and do the hosting in your own application you can do so, too. You just need to reference `servicehost.exe`. Then, in your code, start the Service Host like so:

```
using servicehost;
...
using(var host = new ServiceHost()) {
	host.Start(new Uri("http://localhost:1234"));
	// do what you have to do to keep the process alive
}
```

In case you cannot wrap the Service Host instance with a `using` you should call `Stop()` at the end:

```
using servicehost;
...
var host = new ServiceHost();
host.Start(new Uri("http://localhost:1234"));
...
host.Stop();
```
## Calling a Service
Calling a service hosted by Service Host is straightforward using basic classes from the .NET base class library. But feel free to check out [RestSharp](http://restsharp.org/) for example.

Here's how you can do it if you want to pass the input via querystring parameters:

```
// Service
using servicehost.contract;
...
[Service]
public class SimpleService {
  [EntyPoint(HttpMethods.Get, "/add", InputSources.Querystring)]
  public string Add(string input) {
    ...
  }
}

// Client
using System.Net;
...
var output = new WebClient().DownloadString("http://localhost:1234/add?A=3&B=4");
```

`output` will contain the output JSON, e.g. `{"Sum":7}`.

In case you want to provide the input with the HTTP payload, though, you have to build the JSON yourself:

```
// Service
using servicehost.contract;
...
[Service]
public class SimpleService {
  [EntyPoint(HttpMethods.Get, "/add", InputSources.Payload)]
  public string Add(string input) {
    ...
  }
}

// Client
var cli = new WebClient();
cli.Headers.Add("Content-Type", "application/json");
var input = "{\"A\":3, \"B\":4}";
var output = cli.UploadString("http://localhost:1234/add", "Get", input);
```

Make sure to set the `Content-Type` to `application/json` and provide the right HTTP method with the call to `UploadString()`.

### Exception Handling
Any exceptions happening during service execution should be handled by your service side code gracefully and be encoded in the JSON output. Service Host does not provide any special way to carry exceptions back to the client.

However, if you fail to catch an exception Service Host will catch it and pass it back to the client as a textual payload marked with the HTTP status code 500. You might want to check for that, because the `WebClient` will throw a client side exception in that case.

```
try
{
    var cli = new WebClient();
    var output = cli.DownloadString("http://localhost:1234/add?A=3&B=4");
    ...
}
catch (WebException webex) {
    var resp = (HttpWebResponse)webex.Response;
    if (resp.StatusCode == HttpStatusCode.InternalServerError) {
      var readStream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
      var exceptionMessage = readStream.ReadToEnd();
      ...
    }
    else
      ...
}
```

## Static Content
Service Host does not just provide access to hosted service logic but also to static content like HTML pages or images.

Static content just needs to be stored in a directory called `content` in the Service Host folder. When a service is accessed via `http://localhost:1234/add` then static content can be reached like so: `http://localhost:1234/content/helloworld.html`.

Check out the demo client:

* In the demo client folder start the Service Host: `mono servicehost.exe http://localhost:1234`
* Open your browser and point it to `http://localhost:1234/content/addition.html`
* Enter two numbers to add and hit the button. The page will access a service coming with the demo client at `http://localhost:1234/add` and display the result.