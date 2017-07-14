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
            using (var sut = new ServiceHost()) {
                var endpoint = new Uri("http://localhost:1234");
                sut.Start(endpoint);

                // plain WebClient
                Console.WriteLine("GET reverse");
                var cli = new WebClient();
                var result = cli.DownloadString("http://localhost:1234/reverse?Text=hello");
                Console.WriteLine(result);

                // plain WebClient
                Console.WriteLine("POST add");
                cli.Headers.Add("Content-Type", "application/json");
                result = cli.UploadString("http://localhost:1234/add", "Post", "{\"A\":3, \"B\":4}");
                Console.WriteLine(result);

                // plain WebClient
                Console.WriteLine("GET now");
                result = cli.DownloadString("http://localhost:1234/now");
                Console.WriteLine(result);

                // plain WebClient
                Console.WriteLine("GET echo");
                result = cli.DownloadString("http://localhost:1234/echo?ping=$datetime");
                Console.WriteLine(result);

                // RestSharp
                var client = new RestClient(endpoint);
                var request = new RestRequest("/add", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new AddRequest { A=2, B=3 });

                IRestResponse<AddResult> response = client.Execute<AddResult>(request);
                Assert.AreEqual(5, response.Data.result);
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