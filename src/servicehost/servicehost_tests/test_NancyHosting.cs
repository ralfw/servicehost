using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using servicehost;
using servicehost.nonpublic;
using servicehost.nancy;
using RestSharp;

namespace servicehost_tests
{
    [TestFixture]
    public class test_NancyHosting
    {
        [Test, Explicit]
        public void Start_request_stop()
        {
            using (var sut = new NancyHosting())
            {
                Console.WriteLine("Started...");
                sut.Start(new Uri("http://localhost:1234"), new ServiceInfo[0]);
                Console.WriteLine("Started!");

                var result = new WebClient().DownloadString("http://localhost:1234/acceptancetest_helloworld");
                Console.WriteLine("Called...");
                Assert.AreEqual("Hello World!", result);
                Console.WriteLine("Received: {0}", result);
            }
        }

        [Test, Explicit]
        public void Host_hard_coded_service() { 
            using (var sut = new NancyHosting())
            {
                Console.WriteLine("Started...");
                sut.Start(new Uri("http://localhost:1234"), new[] { 
                    new ServiceInfo{
                        ServiceType = typeof(NancyHostingTestService),
                        EntryPointMethodname = "Echo",
                        Route = "/echo_querystring",
                        HttpMethod = HttpMethods.Get,
                        InputSource = InputSources.Querystring
                    }
                });
                Console.WriteLine("Started with service!");

                var cli = new WebClient();
                var result = cli.DownloadString("http://localhost:1234/echo_querystring?payload=$data");

                Console.WriteLine("Called...");
                Assert.AreEqual(new[] { "handle" }, NancyHostingTestService.Log.ToArray());
                Assert.AreEqual("{\"payload\":\"42\"}", result.Replace(" ", ""));
                Console.WriteLine("Received: {0}", result);
            }
        }

        [Test, Explicit]
        public void Host_hard_coded_service_with_setup_and_teardown()
        {
            using (var sut = new NancyHosting())
            {
                Console.WriteLine("Started...");
                sut.Start(new Uri("http://localhost:1234"), new[] {
                    new ServiceInfo{
                        ServiceType = typeof(NancyHostingTestService),
                        EntryPointMethodname = "Echo",
                        SetupMethodname = "Setup",
                        TeardownMethodname = "Cleanup",
                        Route = "/echo_payload",
                        HttpMethod = HttpMethods.Post,
                        InputSource = InputSources.Payload
                    }
                });
                Console.WriteLine("Started with service!");

                var cli = new WebClient();
                cli.Headers.Add("Content-Type", "application/json");
                var result = cli.UploadString("http://localhost:1234/echo_payload", "Post", "{\"payload\":\"$data\"}");

                Console.WriteLine("Called...");
                Assert.AreEqual(new[] { "setup", "handle", "cleanup" }, NancyHostingTestService.Log.ToArray());
                Assert.AreEqual("{\"payload\":\"42\"}", result.Replace(" ", ""));
                Console.WriteLine("Received: {0}", result);
            }
        }


        [Test, Explicit]
        public void Host_hard_coded_service_with_exception()
        {
            using (var sut = new NancyHosting())
            {
                Console.WriteLine("Started...");
                sut.Start(new Uri("http://localhost:1234"), new[] {
                    new ServiceInfo{
                        ServiceType = typeof(NancyHostingTestService),
                        EntryPointMethodname = "XYZ",
                        Route = "/echo_querystring",
                        HttpMethod = HttpMethods.Get,
                        InputSource = InputSources.Querystring
                    }
                });
                Console.WriteLine("Started with service!");

                try
                {
                    var cli = new WebClient();
                    cli.DownloadString("http://localhost:1234/echo_querystring?payload=$data");
                }
                catch (WebException webex) {
                    //TODO: how to get at the exception message from the server?
                    Console.WriteLine("*** {0}", webex);                    
                }
            }
        }
    }

    public class NancyHostingTestModule : Nancy.NancyModule
    {
        public NancyHostingTestModule()
        {
            Get["/acceptancetest_helloworld"] = _ => "Hello World!";
        }
    }

    public class NancyHostingTestService {
        public static List<string> Log;

        public NancyHostingTestService() { 
            Log = new List<string>();
        }

        public void Setup() { Log.Add("setup"); }
        public void Cleanup() { Log.Add("cleanup"); }
        public string Echo(string json) {
            Log.Add("handle");
            return json.Replace("$data", "42");
        }
    }
}