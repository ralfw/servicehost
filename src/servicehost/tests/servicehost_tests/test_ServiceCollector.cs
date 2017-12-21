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
using HttpMethods = servicehost.contract.HttpMethods;

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

            var services = sut.Collect().ToArray();
            
            Assert.AreEqual(16, services.Length);
            
            var service = services.Where(s => s.ServiceType.Name.IndexOf("MyService") >= 0 && s.EntryPointMethodname == "Echo").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Get, service.HttpMethod);
            Assert.AreEqual("Arrange", service.SetupMethodname);
            Assert.AreEqual("Cleanup", service.TeardownMethodname);
            Assert.AreEqual("/echo", service.Route);

            Assert.AreEqual(1, service.Parameters.Length);
            Assert.AreEqual("ping", service.Parameters[0].Name);
            Assert.AreSame(typeof(string), service.Parameters[0].Type);

            Assert.AreEqual(typeof(string), service.ResultType);


            service = services.Where(s => s.ServiceType.Name.IndexOf("MyService") >= 0 && s.EntryPointMethodname == "JsonDeser").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Post, service.HttpMethod);
            Assert.AreEqual("Arrange", service.SetupMethodname);
            Assert.AreEqual("Cleanup", service.TeardownMethodname);
            Assert.AreEqual("/reflection/{id}", service.Route);

            Assert.AreEqual(2, service.Parameters.Length);
            Assert.AreEqual("id", service.Parameters[0].Name);
            Assert.AreSame(typeof(string), service.Parameters[0].Type);
            Assert.IsFalse(service.Parameters[0].IsPayload);
            Assert.AreEqual("payload", service.Parameters[1].Name);
            Assert.IsTrue(service.Parameters[1].Type.Name.IndexOf("JsonPayload")>=0);
            Assert.IsTrue(service.Parameters[1].IsPayload);

            Assert.IsTrue(service.ResultType.Name.IndexOf("JsonPayload") >= 0);


            service = services.Where(s => s.ServiceType.Name.IndexOf("YourService") >= 0 && s.EntryPointMethodname == "Add").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Post, service.HttpMethod);
            Assert.AreEqual(null, service.SetupMethodname);
            Assert.AreEqual(null, service.TeardownMethodname);
            Assert.AreEqual("/add", service.Route);
            Assert.IsTrue(service.ResultType.Name.IndexOf("AddResult") >= 0);


            service = services.Where(s => s.ServiceType.Name.IndexOf("MyService") >= 0 && s.EntryPointMethodname == "Deafmute").FirstOrDefault();
            Assert.AreEqual(servicehost.nonpublic.HttpMethods.Get, service.HttpMethod);
            Assert.AreEqual("Arrange", service.SetupMethodname);
            Assert.AreEqual("Cleanup", service.TeardownMethodname);
            Assert.AreEqual("/deafmute", service.Route);
            Assert.AreEqual(0, service.Parameters.Length);
            Assert.AreSame(service.ResultType, typeof(void));
        }
        
        
        [Test]
        public void Collect_explicit()
        {
            var sut = new ServiceCollector();

            var services = sut.Collect(new[]{typeof(SomeTestService), typeof(test_ServiceCollector)}).ToArray();
            
            Assert.AreEqual(17, services.Length);
            
            var service = services.Where(s => s.ServiceType.Name.IndexOf("SomeTestService") >= 0 && s.EntryPointMethodname == "f").FirstOrDefault();
            Assert.IsNotNull(service);
            
            service = services.Where(s => s.ServiceType.Name.IndexOf("test_ServiceCollector") >= 0).FirstOrDefault();
            Assert.IsNull(service);
        }


        [Service]
        class SomeTestService
        {
            [EntryPoint(HttpMethods.Get, "/test")]
            public void f() {}
        }
    }
}