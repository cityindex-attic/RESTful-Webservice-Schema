using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using JsonSchemaGeneration.JsonSchemaDTO;
using JsonSchemaGeneration.WcfSMD;
using Newtonsoft.Json;
using NUnit.Framework;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{

    [TestFixture]
    public class ScratchPad
    {
        [Test]
        public void LoadWCF()
        {
            var asm = Assembly.LoadFrom("RESTWebServicesDTO.dll");

            var config = XDocument.Load("web.config");
            var apiNode = config.XPathSelectElement("configuration/tradingApi");
            var profile = apiNode.XPathSelectElement("profiles").Descendants("profile").First();
            var dtoAssemblyNames = profile.Descendants("dtoAssemblies").Descendants("add").Select(n => n.Attribute("assembly").Value).ToArray();

            var routeNodes = profile.XPathSelectElement("routes").XPathSelectElements("add").ToList();
            List<UrlMapElement > routes = new List<UrlMapElement>();
            foreach (var item in routeNodes)
            {
                UrlMapElement map = new UrlMapElement()
                                     {
                                         Endpoint = item.Attribute("endpoint").Value + item.Attribute("pathInfo").Value,
                                         Name = item.Attribute("name").Value,
                                         Type = item.Attribute("type").Value
                                     };
                routes.Add(map);
            }

        }

        [Test]
        public void Test()
        {
            var assemblyNames = new[]{
               "TradingApi.CoreDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
               "RESTWebServicesDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
           };

            var auditor = new Auditor();
            XmlDocUtils.EnsureXmlDocsAreValid(assemblyNames);
            auditor.AuditTypes(assemblyNames);

        }



        [Test]
        public void Test2()
        {
            var assemblyNames = new[]{
               "TradingApi.CoreDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
               "RESTWebServicesDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
           };

            XmlDocUtils.EnsureXmlDocsAreValid(assemblyNames);

            var emitter = new JsonSchemaDtoEmitter();

            var result = emitter.EmitDtoJson(assemblyNames);
            Console.WriteLine(result);

        }














        //[Test]
        //public void CheckJsonNetProgressInJSchema()
        //{
        //    var gen = new Newtonsoft.Json.Schema.JsonSchemaGenerator();
        //    var schema = gen.Generate(typeof(MetadataProcessor.DTOTestAssembly.Class1));
        //    Console.WriteLine(schema.ToString());
        //    // fairly capable implementation for simple use cases. would take a lot of work
        //    // to get it to work for us right now. keep an eye on it.

        //}
    }
}
