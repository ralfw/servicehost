# Service Hosting

Der Service Host sucht in seinem Verzeichnis nach Assemblies, in denen Klassen stehen, die in bestimmter Weise mit Attributen annotiert sind:


[Service]
public class MyService {
	[Setup]
	public void Setup() {}

	[Teardown]
	public void Cleanup() {}


	[EntryPoint(Methods.Get, "/projects", Inputsources.Querystring)]
	public string ListProjects(string) {...}

	[EntryPoint(Methods.Post, "/projects")]
	// [EntryPoint(Methods.Post, "/projects", Inputsources.Payload)]
	public string ListProjects(string) {...}
}


public class ServiceHostModule : Nancy.NancyModule
{
    public ServiceHostModule()
    {
        Get["/projects"] = p => {
        	var service = new MyService();
        	service.Setup();
        	var input = MapQuerystringToInput(p);
        	var output = service.ListProjects(input);
        	service.Cleanup();
        	return MapOutput(output);
        };

        Post["/projects"] = p => {
        	var service = new MyService();
        	service.Setup();
        	var input = MapPayloadToInput(p);
        	var output = service.ListProjects(input);
        	service.Cleanup();
        	return MapOutput(output);
        };
    }
}


Services stellen einen oder mehrere Entrypoints bereit, über die man sie betreten kann. Input und Output sind JSON Nachrichten.
Wie Entrypoints von außen erreicht werden sollen, legen sie selbst fest. Dazu gehört, woher ihr Input kommt: der kann aus dem Querystring ermittelt
werden oder aus der Http Request Payload bestehen (default).

### Flow
1. Assemblies sammeln und laden
2. Service klassen sammeln
3. Nancy Modul generieren, das alle Services zusammenfasst (kann als Quelltext erzeugt werden)
4. Nancy server starten (mit generiertem Modul)

Es sollte einen Standardservice geben, der eine Selbstauskunft über den Service Host zugänglich macht.

## Usage

servicehost.exe <ip-address>

servicehost.exe http://localhost:1234

Service Assemblies im selben Verzeichnis!