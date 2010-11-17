using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TradingApi.Configuration;

namespace MetadataProcessor.Tests
{
    ///<summary>
    ///</summary>
    [TestFixture]
    public class SmdFixture
    {
        public static string GetTestTarget(string name)
        {
            return File.ReadAllText(@"..\..\test-targets\" + name + ".txt");
        }

        [Test, Ignore]
        public void BuildTestService()
        {
            var profile = TradingApiConfigurationSection.Instance.Profiles[""];
            const string smdUrl = "http://tempuri.com/";

            string smdTargetUrl = smdUrl.Substring(0, smdUrl.LastIndexOf('/') + 1);

            string smdSchemaUrl = smdTargetUrl + "schema";
            string apiVersion = profile.Version;
            string smdDescription = "City Index RESTful API " + apiVersion;
            const string smdVersion = "2.0";

            var smdBase = SmdGenerator.BuildSMDBase(smdUrl, smdTargetUrl, smdSchemaUrl, smdDescription, apiVersion, smdVersion, true);

            var dtoAssemblies = profile.DtoAssemblies.Cast<AssemblyReferenceElement>().ToList();

            var mappedTypes = new List<Type>(); // just to keep track of types so we don't map twice
            foreach (UrlMapElement route in profile.Routes)
            {
                SmdGenerator.BuildServiceMapping(route, mappedTypes, smdBase, dtoAssemblies, true);
            }

            // TODO: set caching headers and use asp.net cache

            var smdJson = smdBase.ToString();
            Assert.AreEqual(GetTestTarget("BuildTestService"), smdJson);
        }
    }
}
