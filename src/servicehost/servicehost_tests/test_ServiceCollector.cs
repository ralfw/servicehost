using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using servicehost;
using servicehost.nonpublic;
using servicehost.nancy;
using servicehost.contract;
using System.Reflection;
using System.IO;

namespace servicehost_tests
{

    [TestFixture]
    public class test_ServiceCollector
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void Collect()
        {
            var sut = new ServiceCollector();

            var services = sut.Collect();

            var service = services.Where(s => s.ServiceType.Name.IndexOf("MyService") >= 0 && s.EntryPointMethodname == "Echo").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Get, service.HttpMethod);
            Assert.AreEqual(servicehost.nonpublic.InputSources.Querystring, service.InputSource);
            Assert.AreEqual("Arrange", service.SetupMethodname);
            Assert.AreEqual("Cleanup", service.TeardownMethodname);
            Assert.AreEqual("/echo", service.Route);

            service = services.Where(s => s.ServiceType.Name.IndexOf("MyService") >= 0 && s.EntryPointMethodname == "JsonDeser").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Get, service.HttpMethod);
            Assert.AreEqual(servicehost.nonpublic.InputSources.Payload, service.InputSource);
            Assert.AreEqual("Arrange", service.SetupMethodname);
            Assert.AreEqual("Cleanup", service.TeardownMethodname);
            Assert.AreEqual("/reflection", service.Route);

            service = services.Where(s => s.ServiceType.Name.IndexOf("YourService") >= 0 && s.EntryPointMethodname == "Add").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Post, service.HttpMethod);
            Assert.AreEqual(servicehost.nonpublic.InputSources.Payload, service.InputSource);
            Assert.AreEqual(null, service.SetupMethodname);
            Assert.AreEqual(null, service.TeardownMethodname);
            Assert.AreEqual("/add", service.Route);
        }
    }
}