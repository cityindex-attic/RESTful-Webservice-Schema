using System;
using System.Collections.Generic;
using MetadataGeneration.Core.JsonSchemaDTO;
using NUnit.Framework;
using TradingApi.Configuration;

namespace MetadataGeneration.Core.Tests
{
    [TestFixture]
    public class SMDEmitterTests: GeneratorTestsBase 
    {
        [Test, Ignore("WIP")]
        public void ExcludeTagShouldOmitServiceMethod()
        {
            var xmlDocSource = new XmlDocSource();
            xmlDocSource.Dtos.Add(AssemblyWithXmlDocs.CreateFromName("TestAssembly.DTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "."));
            xmlDocSource.RouteAssembly = AssemblyWithXmlDocs.CreateFromName("TestAssembly.Service, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ".");
            xmlDocSource.Routes = new List<UrlMapElement>{
                new UrlMapElement {
                    Endpoint = "/excluded",
                    Name = "excluded",
                    Type = "TestAssembly.Service.IExcludedService, TestAssembly.Service, Version=1.0.0.0"}};

            var jsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(xmlDocSource);
            var smdSchema = new WcfSMD.Emitter().EmitSmdJson(xmlDocSource,true,jsonSchema);

            Console.WriteLine(smdSchema.SMD);
//            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["IEnumerableOfInt"]["type"].ToString(), Is.EqualTo("array"));
//            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["ArrayOfInt"]["type"].ToString(), Is.EqualTo("array"));
//            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["ListOfInt"]["type"].ToString(), Is.EqualTo("array"));
//            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["IListOfInt"]["type"].ToString(), Is.EqualTo("array"));
        }

        [Test]
        public void CorrectlyReferencedServicesShouldBeIncludedInSMD()
        {
            var xmlDocSource = new XmlDocSource();
            xmlDocSource.Dtos.Add(AssemblyWithXmlDocs.CreateFromName("TestAssembly.DTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "."));
            xmlDocSource.RouteAssembly = AssemblyWithXmlDocs.CreateFromName("TestAssembly.Service, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ".");
            xmlDocSource.Routes = new List<UrlMapElement>{ 
                new UrlMapElement {
                    Endpoint = "/session",
                    Name = "session",
                    Type = "TestAssembly.Service.ITestService, TestAssembly.Service, Version=1.0.0.0"},
                new UrlMapElement {
                    Endpoint = "/session",
                    Name = "session_logout",
                    Type = "TestAssembly.Service.ITestService, TestAssembly.Service, Version=1.0.0.0"}
            };

            var jsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(xmlDocSource);
            var smdSchema = new WcfSMD.Emitter().EmitSmdJson(xmlDocSource, true, jsonSchema);

            Console.WriteLine(smdSchema.SMD);
            Assert.That(smdSchema.SMD["services"]["rpc"]["services"]["CreateSession"], Is.Not.Null);
            Assert.That(smdSchema.SMD["services"]["rpc"]["services"]["DeleteSession"], Is.Not.Null);
        }
    }
}
