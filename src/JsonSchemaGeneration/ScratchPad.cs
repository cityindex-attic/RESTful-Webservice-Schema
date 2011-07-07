using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JsonSchemaGeneration
{

    [TestFixture]
    public class ScratchPad
    {

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

            var result = emitter.EmitDtoJson("NameSpaceName", assemblyNames);
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
