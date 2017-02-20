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
                var cli = new WebClient();
                cli.Headers.Add("Content-Type", "application/json");
                var result = cli.UploadString("http://localhost:1234/add", "Post", "{\"A\":3, \"B\":4}");
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
    }
}