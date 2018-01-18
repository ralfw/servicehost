using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Nancy;
using Nancy.Extensions;
using servicehost.contract;
using servicehost.nancy.nonpublic;
using servicehost.nonpublic;
using HttpMethods = servicehost.nonpublic.HttpMethods;

namespace servicehost.nancy
{
    public class NancyServiceModule : Nancy.NancyModule
    {
        public NancyServiceModule(IEnumerable<ServiceInfo> services)
        {
            Options[@"^(.*)$"] = p => CORS.Handle_preflight_request(Request);
            
            foreach (var service in services)
                Register_service(service);
        }

        void Register_service(ServiceInfo service) {
            var handler = Create_handler(service);
            Register_route(service.HttpMethod, service.Route, handler);
        }

        Func<dynamic, dynamic> Create_handler(ServiceInfo service) {
            return (dynamic param) => {
                try {
                    var input = Get_input(param, service);

                    var handler = new ServiceAdapter(service.ServiceType, service.EntryPointMethodname, service.SetupMethodname, service.TeardownMethodname);
                    var output = handler.Execute(input);

                    var response = Produce_output(output);
                    return CORS.Handle_simple_request(Request, response);
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

        object[] Get_input(dynamic routeParams, ServiceInfo service) {
            return service.Parameters.Select(p => Get_input_from_param(routeParams, Request.Query, p))
                                     .ToArray();
        }

        object Get_input_from_param(Nancy.DynamicDictionary routeParams, Nancy.DynamicDictionary querystringParams, ServiceParameter param) {
            if (param.IsPayload) {
                if (param.Type == typeof(string))
                    return this.Request.Body.AsString();
                
                if (this.Request.Headers["Content-Type"].All(h => h != "application/json"))
                    throw new InvalidOperationException("Invalid Content-Type for payload data! Needs to be 'application/json'.");
                
                if (param.Type == typeof(JsonData))
                    return new JsonData(this.Request.Body.AsString());
                
                var payloadJson = this.Request.Body.AsString();
                return new JavaScriptSerializer().Deserialize(payloadJson, param.Type);
            }
            
            if (Is_in(routeParams))
                return Parse(routeParams[param.Name].ToString());
                
            if (Is_in(querystringParams))
                return Parse(querystringParams[param.Name].ToString());

            throw new InvalidOperationException($"Parameter not found in route or query string: {param.Name}!");


            bool Is_in(Nancy.DynamicDictionary paramList) => paramList.ContainsKey(param.Name);

            object Parse(string data) {
                if (param.Type == typeof(string))
                    return data;
                if (param.Type == typeof(Guid))
                    return new Guid(data);
                if (param.Type == typeof(DateTime))
                    return DateTime.Parse(data);
                if (param.Type == typeof(JsonData))
                    return new JsonData(data);
                var json = new JavaScriptSerializer();
                return json.Deserialize(data, param.Type);
            }
        }


        Response Produce_output(object obj) {
            if (obj is string) {
                var response = (Response)(obj.ToString());
                response.ContentType = "text/plain";
                return response;
            }
            else {
                var objJson = obj is JsonData data ? data.Data
                                                   : new Nancy.Json.JavaScriptSerializer().Serialize(obj);
                var response = (Response)objJson;
                response.ContentType = "application/json";
                return response;
            }
        }
    }


    static class CORS {
        public static Response Handle_preflight_request(Request req) {
            var resp = new Response {StatusCode = HttpStatusCode.OK};
            resp.Headers.Add("Access-Control-Allow-Origin", "*");
            resp.Headers.Add("Access-Control-Max-Age", "86400");

            var corsheader = req.Headers["Access-Control-Request-Method"].FirstOrDefault();
            if (corsheader != null)
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");

            corsheader = req.Headers["Access-Control-Request-Headers"].FirstOrDefault();
            if (corsheader != null)
                resp.Headers.Add("Access-Control-Allow-Headers", corsheader);
            
            return resp;
        }
        
        public static Response Handle_simple_request(Request req, Response response) {
            var origin = req.Headers["Origin"].FirstOrDefault();
            if (origin != null)
                response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }
    }
}