using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nancy;
using Nancy.Extensions;

namespace servicehost.nonpublic.nancy
{

    public class NancyServiceModule : Nancy.NancyModule
    {
        public NancyServiceModule(IEnumerable<ServiceInfo> services) {
            foreach (var service in services)
                Register_service(service);
        }

        void Register_service(ServiceInfo service) {
            var handler = Create_handler(service);
            Register_route(service.HttpMethod, service.Route, handler);
        }

        Func<dynamic, dynamic> Create_handler(ServiceInfo service) {
            return (dynamic param) => {
                try
                {
                    var input = Get_input(service.InputSource);
                    var handler = new ServiceAdapter(service.ServiceType, service.EntryPointMethodname, service.SetupMethodname, service.TeardownMethodname);
                    var output = handler.Execute(input);
                    return Response.AsText(output, "application/json");
                }
                catch (Exception ex) {
                    var resp = (Response)($"ServiceHost: Service request could not be handled! Exception: {ex}");
                    resp.StatusCode = HttpStatusCode.InternalServerError;
                    return resp;
                }
            };
        }

        void Register_route(HttpMethods method, string route, Func<dynamic, dynamic> handler) {
            switch (method) {
                case HttpMethods.Get:
                    Get[route] = handler;
                    break;
                case HttpMethods.Post:
                    Post[route] = handler;
                    break;
                case HttpMethods.Put:
                    Put[route] = handler;
                    break;
                case HttpMethods.Delete:
                    Delete[route] = handler;
                    break;
            }
        }

        string Get_input(InputSources source) { 
            switch (source)
            {
                case InputSources.Payload:
                    if (!this.Request.Headers["Content-Type"].Any(h => h == "application/json"))
                        throw new InvalidOperationException("Invalid Content-Type for payload data! Needs to be 'application/json'.");
                    return this.Request.Body.AsString();
                default:
                    return new Nancy.Json.JavaScriptSerializer().Serialize((IDictionary<string, object>)this.Request.Query);
            }
        }
    }
}