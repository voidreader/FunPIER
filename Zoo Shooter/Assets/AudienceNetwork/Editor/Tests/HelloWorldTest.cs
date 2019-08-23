using System;
using NUnit.Framework;
using AudienceNetwork;

namespace helloworld_project
{
    [TestFixture]
    public class EmptyClassTest
    {
        [Test]
        public void testHelloWorld()
        {
            EmptyClass empty = new EmptyClass();
            Assert.AreEqual("Hello World!", empty.helloWorld());
        }

        [Test]
        public void testAudienceNetworkSDKVersion()
        {
            Console.WriteLine("Currrent AudienceNetwork SDK version is " + SdkVersion.Build);
        }

        // [Test]
        // public void testHelloWorld2()
        // {
        //     EmptyClass empty = new EmptyClass();
        //     Assert.AreEqual ("Hello World!!", empty.helloWorld ());
        // }
        //
        // [Test]
        // public void testHelloWorld3()
        // {
        //     Assert.Ignore();
        // }
    }
}
