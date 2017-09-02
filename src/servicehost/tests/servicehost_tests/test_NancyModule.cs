using System;
using CsQuery.ExtensionMethods;
using Nancy;
using Nancy.Testing;
using NUnit.Framework;
using servicehost;

namespace servicehost_tests
{
    [TestFixture]
    public class test_NancyModule
    {
        [Test]
        public void Test_dynamically_created_module_with_Nancy_testing()
        {
            var sut = ServiceModuleFactory.Build();
            var browser = new Browser(with => with.Module(sut));

            var response = browser.Get("/now");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Body.AsString().IndexOf(DateTime.Now.Year.ToString())>=0);
        }
    }
}