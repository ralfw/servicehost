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
                try {
                    var input = Get_input(param, service);

                    var handler = new ServiceAdapter(service.ServiceType, service.EntryPointMethodname, service.SetupMethodname, service.TeardownMethodname);
                    var output = handler.Execute(input);

                    var response = Produce_output(output);
                    return Enable_CORS(response);
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
                else {
                    if (this.Request.Headers["Content-Type"].All(h => h != "application/json"))
                        throw new InvalidOperationException("Invalid Content-Type for payload data! Needs to be 'application/json'.");
                    
                    if (param.Type == typeof(JsonData))
                        return new JsonData(this.Request.Body.AsString());
                    
                    var payloadJson = this.Request.Body.AsString();
                    return new JavaScriptSerializer().Deserialize(payloadJson, param.Type);
                }
            }
            else if (Is_in(routeParams)) {
                return Parse(routeParams[param.Name].ToString());
            }
            else if (Is_in(querystringParams)) {
                return Parse(querystringParams[param.Name].ToString());
            }
            throw new InvalidOperationException($"Parameter not found in route or query string: {param.Name}!");


            bool Is_in(Nancy.DynamicDictionary paramList) => paramList.ContainsKey(param.Name);

            object Parse(string data) {
                if (param.Type == typeof(string))
                    return data;
                else if (param.Type == typeof(Guid)) {
                    return new Guid(data);
                }
                else if (param.Type == typeof(DateTime)) {
                    return DateTime.Parse(data);
                }
                else if (param.Type == typeof(JsonData))
                    return new JsonData(data);
                else {
                    var json = new JavaScriptSerializer();
                    return json.Deserialize(data, param.Type);
                }
            }
        }


        Response Produce_output(object obj) {
            if (obj is string) {
                var response = (Response)(obj.ToString());
                response.ContentType = "text/plain";
                return response;
            }
            else {
                var objJson = obj is JsonData ? ((JsonData) obj).Data
                                              : new Nancy.Json.JavaScriptSerializer().Serialize(obj);
                var response = (Response)objJson;
                response.ContentType = "application/json";
                return response;
            }
        }


        Response Enable_CORS(Response response) {
            var origin = Request.Headers["Origin"].FirstOrDefault();
            if (origin != null)
                response.Headers.Add("Access-Control-Allow-Origin", origin);
            return response;
        }
    }
}