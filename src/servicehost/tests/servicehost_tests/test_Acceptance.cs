using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using servicehost;
using servicehost.nonpublic;
using servicehost.nancy;
using RestSharp;
using System.IO;

namespace servicehost_tests
{
    public class AddRequest {
        public int A { get; set; }
        public int B { get; set; }
    }

    public class AddResult {
        public int result { get; set; }
    }


    [TestFixture]
    public class test_Acceptance {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void Start_call_stop() {
            Console.WriteLine("---Start call stop---");

            using (var sut = new ServiceHost()) {
                var endpoint = new Uri("http://localhost:1234");
                sut.Start(endpoint);

                // plain WebClient (yourservice)
                Console.WriteLine("GET reverse");
                var cli = new WebClient();
                var result = cli.DownloadString("http://localhost:1234/reverse?Text=hello");
                Console.WriteLine(result);

                // plain WebClient (yourservice)
                Console.WriteLine("POST add");
                cli.Headers.Add("Content-Type", "application/json");
                result = cli.UploadString("http://localhost:1234/add", "Post", "{\"A\":3, \"B\":4}");
                Console.WriteLine(result);

                // plain WebClient (myservice)
                Console.WriteLine("GET now");
                result = cli.DownloadString("http://localhost:1234/now");
                Console.WriteLine(result);

                // plain WebClient (myservice)
                Console.WriteLine("GET deafmute");
                result = cli.DownloadString("http://localhost:1234/deafmute");
                Console.WriteLine("deafmute result: <{0}>", result);

                // plain WebClient (myservice)
                Console.WriteLine("GET echo");
                result = cli.DownloadString("http://localhost:1234/echo?ping=$datetime");
                Console.WriteLine(result);

                // RestSharp (yourservice)
                var client = new RestClient(endpoint);
                var request = new RestRequest("/add", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new AddRequest { A=2, B=3 });

                IRestResponse<AddResult> response = client.Execute<AddResult>(request);
                Assert.AreEqual(5, response.Data.result);
            }
        }


        [Test]
        public void JsonData_in_and_out() {
            Console.WriteLine("---JsonData in and out---");

            using (var sut = new ServiceHost())
            {
                var endpoint = new Uri("http://localhost:1234");
                sut.Start(endpoint);

                var cli = new WebClient();
                cli.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = cli.UploadString("http://localhost:1234//jsondata?a={\"n\":42}", "{\"pi\":3.14}");
                Assert.AreEqual("{\n{\"n\":42}\n{\"pi\":3.14}\n}", result);
                Console.WriteLine(result);
            }
        }

        
        [Test]
        public void Param_type_mapping() {
            Console.WriteLine("---Param type mapping---");

            using (var sut = new ServiceHost())
            {
                var endpoint = new Uri("http://localhost:1234");
                sut.Start(endpoint);

                // plain WebClient (yourservice)
                Console.WriteLine("GET paramtypes");
                var cli = new WebClient();
                cli.DownloadString("http://localhost:1234//paramtypes?s=hello&i=42&b=true&d=3.14&f=3.1415&de=3.141592&i32=99&id=a1f5ca8a-9fed-498c-9cb7-a5627c49e779&da=22.8.2017");
            }
        }


        [Test]
        public void Serve_static_content() {
            const string ENDPOINT = "http://localhost:1234";
            const string RESULT_FILENAME = "result.txt";

            using (var sut = new ServiceHost())
            {
                var endpoint = new Uri(ENDPOINT);
                sut.Start(endpoint);
                File.Delete(RESULT_FILENAME);

                var cli = new WebClient();
                cli.DownloadFile(ENDPOINT + "/content/helloworld.html", RESULT_FILENAME);

                var result = File.ReadAllText(RESULT_FILENAME);
                Assert.AreEqual("<html><body>Hello, World!</body></html>", result);
            }
        }
    }
}