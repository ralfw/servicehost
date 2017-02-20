using System;
using Nancy.Hosting.Self;
using System.Linq;
using Nancy;
using Nancy.Extensions;
using System.Collections.Generic;

namespace servicehost
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var bootstrapper = new TestBootstrapper();

            Console.WriteLine("before new");
            using (var host = new NancyHost(bootstrapper, new Uri("http://localhost:1234")))
            {
                Console.WriteLine("before start");
                host.Start();
                Console.WriteLine("after start");
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
            }
        }
    }


    public class TestBootstrapper : DefaultNancyBootstrapper
    {
        public TestBootstrapper() {
            Console.WriteLine("bootstrapper ctor");
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            Console.WriteLine("bootstrapping");
            base.ConfigureApplicationContainer(container);

            var inj = new TheInjection("/ctorinject");
            container.Register<TheInjection>(inj);
        }
    }



    public interface IInjections { 
        string Route { get; }
        void Log(string msg);
    }


    public class TheInjection : IInjections
    {
        public string Route { get; set; }

        public TheInjection(string route) {
            Console.WriteLine("injection created");
            this.Route = route;
        }

        public void Log(string msg) {
            Console.WriteLine("Logged by injection: " + msg);
        }
    }


    public class DependencyInjection : NancyModule
    {
        public DependencyInjection(TheInjection injection)
        {
            Console.WriteLine("module config");

            Get[injection.Route] = _ =>
            {
                injection.Log(DateTime.Now.ToString());
                return "hello, injection! " + DateTime.Now.ToString();
            };
        }
    }


    public class JsonStuff : Nancy.NancyModule
    {
        public JsonStuff()
        {
            Get["/error"] = _ =>
            {
                var resp = (Response)($"Error msg {DateTime.Now}");
                resp.StatusCode = HttpStatusCode.InternalServerError;
                return resp;
            };

            Get["/"] = _ => {
                foreach (var k in this.Request.Query.Keys)
                    Console.WriteLine($"{k}: {this.Request.Query[k]}");

                var json = new Nancy.Json.JavaScriptSerializer().Serialize((IDictionary<string,object>)this.Request.Query);

                json = json.Replace("$datetime", DateTime.Now.ToString());
                return Response.AsText(json, "application/json");
            };

            Post["/"] = p =>
            {
                var json = this.Request.Body.AsString();
                var contentType = this.Request.Headers["Content-Type"].FirstOrDefault();
                Console.WriteLine("-- {0}", contentType);
                Console.WriteLine(json);
                Console.WriteLine("--");
                json = json.Replace("$datetime", DateTime.Now.ToString());
                return Response.AsText(json, "application/json");
            };
        }
    }
}
