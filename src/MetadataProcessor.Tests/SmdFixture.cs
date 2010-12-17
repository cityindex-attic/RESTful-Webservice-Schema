using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MetadataProcessor.Tests.TestEndpoint;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MetadataProcessor.Tests
{
    ///<summary>
    ///</summary>
    [TestFixture]
    public class SmdFixture
    {
        private JObject _smdBase;
        private List<string> _dtoAssemblyNames;
        private static readonly ServiceRoute SessionRoute = new ServiceRoute { Name = "session", Endpoint = "/session", ServiceType = typeof(ITestService) };
        private static readonly ServiceRoute SessionLogoutRoute = new ServiceRoute { Name = "session_logout", Endpoint = "/session", ServiceType = typeof(ITestService) };

        [SetUp]
        public void SetUp()
        {
            _smdBase = GenerateSmdBase();
            _dtoAssemblyNames = new List<string> { GetType().Assembly.FullName };
        }

        private static JObject GenerateSmdBase()
        {
            const string smdUrl = "http://tempuri.com/";
            string smdTargetUrl = smdUrl.Substring(0, smdUrl.LastIndexOf('/') + 1);
            string smdSchemaUrl = smdTargetUrl + "schema";
            string apiVersion = "1.1";
            string smdDescription = "City Index RESTful API " + apiVersion;
            const string smdVersion = "2.0";

            return SmdGenerator.BuildSMDBase(smdUrl, smdTargetUrl, smdSchemaUrl, smdDescription, apiVersion, smdVersion, true);
        }

        [Test]
        public void BuildTestService()
        {
            var mappedTypes = new List<Type>(); 
            SmdGenerator.BuildServiceMapping(SessionRoute, mappedTypes, _dtoAssemblyNames, _smdBase, true);
            SmdGenerator.BuildServiceMapping(SessionLogoutRoute, mappedTypes, _dtoAssemblyNames, _smdBase, true);

            var smdJson = _smdBase.ToString();
            var testTarget = GetTestTarget("BuildTestService");
            Console.WriteLine(testTarget);
            Console.WriteLine(smdJson);
            Assert.AreEqual(testTarget, smdJson);
        }

        [Test]
        public void BuildTestServiceWithUnversionedAssemblyNames()
        {
            var mappedTypes = new List<Type>();
            var unVersionedDtoAssemblyNames = new List<string> { "MetadataProcessor.Tests" };

            SmdGenerator.BuildServiceMapping(SessionRoute, mappedTypes, unVersionedDtoAssemblyNames, _smdBase, true);
            SmdGenerator.BuildServiceMapping(SessionLogoutRoute, mappedTypes, unVersionedDtoAssemblyNames, _smdBase, true);

            var smdJson = _smdBase.ToString();
            var testTarget = GetTestTarget("BuildTestService");
            Console.WriteLine(testTarget);
            Console.WriteLine(smdJson);
            Assert.AreEqual(testTarget, smdJson);
        }
        

        public static string GetTestTarget(string name)
        {
            return File.ReadAllText(@"..\..\test-targets\" + name + ".txt");
        }

        [Test]
        public void ParameterDescriptionMetadataIsPulledFromMetaDataOfDtos()
        {
            var mappedTypes = new List<Type>();

            SmdGenerator.BuildServiceMapping(SessionRoute, mappedTypes, _dtoAssemblyNames, _smdBase, true);

            var actualParameters = _smdBase.SelectToken("services.CreateSession.parameters[0]").ToString();
            StringAssert.Contains("\"description\": \"Username is case sensitive\"", actualParameters, "description not found in SMD description of parameters");
        }

        [Test]
        public void AddPropertyDescriptionCanGetTextFromParamValue()
        {
            var property = new JObject();
            var paramElement = XElement.Parse(
@"<param name=""userName"" minLength=""6"" maxLength=""20"" demoValue=""CC735158"">Username description came from param.</param>");
            var docElement = XElement.Parse(
@"<member name=""M:MetadataProcessor.Tests.TestEndpoint.ITestService.LogoutFromQueryString(System.String,System.String)"">
  <summary>Should not be used</summary>
  <param name=""userName"" minLength=""6"" maxLength=""20"" demoValue=""CC735158"">Username description came from param.</param>
  <param name=""session"" format=""guid"" minLength=""36"" maxLength=""36"" demoValue=""5998CBE8-3594-4232-A57E-09EC3A4E7AA8"">The session token. May be set as a service parameter or as a request header.</param>
  <returns></returns>
  <smd method=""DeleteSession"" />
</member>");
            SmdGenerator.AddPropertyDescription(property, paramElement, docElement);

            Assert.AreEqual("\"Username description came from param.\"", property.SelectToken("description").ToString());
        }

        [Test]
        public void AddPropertyDescriptionCanGetTextFromJSchemaValue()
        {
            var property = new JObject();
            var jschemaElement = XElement.Parse(
@"<jschema minLength=""6"" maxLength=""20"" demoValue=""3T999"" />");
            var docElement = XElement.Parse(
@"<member name=""P:MetadataProcessor.Tests.TestDTO.Request.LogOnRequestDTO.UserName"">
  <summary>
             Username description came from jschema dto.
            </summary>
  <jschema minLength=""6"" maxLength=""20"" demoValue=""3T999"" />
</member>");
            SmdGenerator.AddPropertyDescription(property, jschemaElement, docElement);

            Assert.AreEqual("\"Username description came from jschema dto.\"", property.SelectToken("description").ToString());
        }

        [Test]
        public void AddPropertyDescriptionOmittedWhenNoDescriptionAvailable()
        {
            var property = new JObject();
            var jschemaElement = XElement.Parse(
@"<jschema minLength=""6"" maxLength=""20"" demoValue=""3T999"" />");
            var docElement = XElement.Parse(
@"<member name=""P:MetadataProcessor.Tests.TestDTO.Request.LogOnRequestDTO.UserName"">
  <summary></summary>
  <jschema minLength=""6"" maxLength=""20"" demoValue=""3T999"" />
</member>");
            SmdGenerator.AddPropertyDescription(property, jschemaElement, docElement);

            Assert.IsNull(property.SelectToken("description"));
        }
    }
}
